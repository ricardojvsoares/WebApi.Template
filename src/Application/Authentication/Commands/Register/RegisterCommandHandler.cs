using Application.Abstractions.Authentication;
using Application.Abstractions.Security;
using Domain.Authorization;
using Domain.Users.Entities;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Authentication.Commands.Register;

public static class RegisterCommandHandler
{
    public static async Task<ErrorOr<RegisterResponse>> HandleAsync(
        RegisterCommand command,
        ILogger logger,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (await userRepository.EmailExistsAsync(command.Email, cancellationToken))
            {
                return Error.Conflict(description: "An account with that email already exists.");
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
            var email = command.Email.Trim();
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = email,
                EmailNormalized = User.NormalizeEmail(email),
                PasswordHash = passwordHasher.Hash(command.Password),
                DisplayName = command.DisplayName.Trim(),
                IsActive = true,
                CreatedBy = userId,
                CreatedAtUtc = nowUtc,
                UpdatedBy = userId,
                UpdatedAtUtc = nowUtc
            };

            await userRepository.AddAsync(
                user,
                cancellationToken);

            // Self-registered accounts get the baseline role, which grants the todo
            // permissions only. Elevated roles are assigned through the Users feature.
            var defaultRole = await roleRepository.GetByNameAsync(
                Roles.User,
                cancellationToken);

            if (defaultRole is null)
            {
                return Error.Unexpected(
                    description: $"The '{Roles.User}' role is missing. Has the reference-data migration run?");
            }

            await roleRepository.AssignToUserAsync(
                user.Id,
                defaultRole.Id,
                cancellationToken);

            var tokens = await TokenIssuer.IssueAsync(
                user,
                userRepository,
                refreshTokenRepository,
                jwtTokenGenerator,
                refreshTokenGenerator,
                nowUtc,
                cancellationToken);

            return new RegisterResponse(
                tokens.AccessToken,
                tokens.AccessTokenExpiresAtUtc,
                tokens.RefreshToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(RegisterCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
