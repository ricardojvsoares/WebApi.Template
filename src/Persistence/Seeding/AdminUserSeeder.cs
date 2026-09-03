using Application.Abstractions.Security;
using Domain.Authorization;
using Domain.Users.Entities;
using Domain.Users.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Persistence.Seeding;

/// <summary>
/// Creates the bootstrap administrator so a freshly migrated database is usable. The
/// admin is not seeded by a migration because the credentials come from configuration,
/// which migrations cannot reach.
/// </summary>
internal sealed class AdminUserSeeder(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IConfiguration configuration,
    TimeProvider timeProvider,
    ILogger<AdminUserSeeder> logger)
{
    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var email = configuration["Auth:SeedAdmin:Email"];
        var password = configuration["Auth:SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogInformation(
                "Skipping administrator seeding because Auth:SeedAdmin:Email or Auth:SeedAdmin:Password is not configured.");

            return;
        }

        if (await userRepository.EmailExistsAsync(email, cancellationToken))
        {
            // Deliberately does not reset the password of an existing account, so a
            // stale value in configuration cannot silently take over a live admin.
            logger.LogInformation(
                "Administrator {Email} already exists; leaving it untouched.",
                email);

            return;
        }

        var adminRole = await roleRepository.GetByNameAsync(
            Roles.Admin,
            cancellationToken);

        if (adminRole is null)
        {
            logger.LogError(
                "Cannot seed the administrator because the '{Role}' role is missing. Has the reference-data migration run?",
                Roles.Admin);

            return;
        }

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var admin = User.Create(
            email.Trim(),
            passwordHasher.Hash(password),
            "Administrator",
            nowUtc);

        await userRepository.AddAsync(
            admin,
            cancellationToken);

        await roleRepository.AssignToUserAsync(
            admin.Id,
            adminRole.Id,
            cancellationToken);

        logger.LogWarning(
            "Seeded administrator {Email} with the '{Role}' role. Change this password before exposing the API.",
            admin.Email,
            Roles.Admin);
    }
}
