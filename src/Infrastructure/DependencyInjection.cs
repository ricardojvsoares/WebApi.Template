using Application.Abstractions.Authentication;
using Application.Abstractions.Security;
using Infrastructure.Authentication;
using Infrastructure.Messaging;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();

        return services;
    }

    /// <summary>
    /// Reads and validates the JWT settings eagerly. A missing or too-short signing key
    /// would otherwise surface as an unhandled exception on the first login attempt.
    /// </summary>
    public static JwtOptions ReadValidatedJwtOptions(
        this IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>() ?? new JwtOptions();

        if (string.IsNullOrWhiteSpace(options.SigningKey))
        {
            throw new InvalidOperationException(
                $"{JwtOptions.SectionName}:{nameof(JwtOptions.SigningKey)} is not configured.");
        }

        if (System.Text.Encoding.UTF8.GetByteCount(options.SigningKey) < JwtOptions.MinimumSigningKeyBytes)
        {
            throw new InvalidOperationException(
                $"{JwtOptions.SectionName}:{nameof(JwtOptions.SigningKey)} must be at least " +
                $"{JwtOptions.MinimumSigningKeyBytes} bytes so it can sign HMAC-SHA256 tokens.");
        }

        if (options.AccessTokenMinutes < 1)
        {
            throw new InvalidOperationException(
                $"{JwtOptions.SectionName}:{nameof(JwtOptions.AccessTokenMinutes)} must be at least 1.");
        }

        if (options.RefreshTokenDays < 1)
        {
            throw new InvalidOperationException(
                $"{JwtOptions.SectionName}:{nameof(JwtOptions.RefreshTokenDays)} must be at least 1.");
        }

        return options;
    }
}
