using Application.Abstractions.Authentication;
using Application.Abstractions.Security;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Authentication.Commands.Login;

public static class LoginCommandHandler
{
    public static async Task<ErrorOr<AuthenticationResponse>> HandleAsync(
        LoginCommand command,
        ILogger logger,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(
                command.Email,
                cancellationToken);

            // A wrong email, a wrong password and a deactivated account all report the same
            // error so the endpoint cannot be used to enumerate registered addresses.
            var invalidCredentials = Error.Unauthorized(
                description: "The email or password is incorrect.");

            if (user is null)
            {
                return invalidCredentials;
            }

            if (!passwordHasher.Verify(command.Password, user.PasswordHash))
            {
                return invalidCredentials;
            }

            if (!user.IsActive)
            {
                return invalidCredentials;
            }

            return await TokenIssuer.IssueAsync(
                user,
                userRepository,
                refreshTokenRepository,
                jwtTokenGenerator,
                refreshTokenGenerator,
                timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(LoginCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
