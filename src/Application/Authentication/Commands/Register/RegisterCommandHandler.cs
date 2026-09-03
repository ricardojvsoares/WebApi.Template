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
    public static async Task<ErrorOr<AuthenticationResponse>> HandleAsync(
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

            var user = User.Create(
                command.Email.Trim(),
                passwordHasher.Hash(command.Password),
                command.DisplayName.Trim(),
                nowUtc);

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

            return await TokenIssuer.IssueAsync(
                user,
                userRepository,
                refreshTokenRepository,
                jwtTokenGenerator,
                refreshTokenGenerator,
                nowUtc,
                cancellationToken);
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
