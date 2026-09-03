using Application.Abstractions.Authentication;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Authentication.Commands.RefreshAccessToken;

public static class RefreshAccessTokenCommandHandler
{
    public static async Task<ErrorOr<AuthenticationResponse>> HandleAsync(
        RefreshAccessTokenCommand command,
        ILogger logger,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var invalidToken = Error.Unauthorized(
                description: "The refresh token is invalid or has expired.");

            var presentedHash = refreshTokenGenerator.Hash(command.RefreshToken);

            var stored = await refreshTokenRepository.GetByTokenHashAsync(
                presentedHash,
                cancellationToken);

            if (stored is null)
            {
                return invalidToken;
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

            if (!stored.IsActive(nowUtc))
            {
                // A token that was already rotated is being replayed, which means a copy
                // leaked. Revoke the whole family so the attacker's chain dies too.
                if (stored.ReplacedByTokenHash is not null)
                {
                    logger.LogWarning(
                        "Replay of a rotated refresh token detected for user {UserId}. Revoking all of their refresh tokens.",
                        stored.UserId);

                    await refreshTokenRepository.RevokeAllForUserAsync(
                        stored.UserId,
                        nowUtc,
                        cancellationToken);
                }

                return invalidToken;
            }

            var user = await userRepository.GetByIdAsync(
                stored.UserId,
                cancellationToken);

            if (user is null || !user.IsActive)
            {
                return invalidToken;
            }

            var response = await TokenIssuer.IssueAsync(
                user,
                userRepository,
                refreshTokenRepository,
                jwtTokenGenerator,
                refreshTokenGenerator,
                nowUtc,
                cancellationToken);

            await refreshTokenRepository.RevokeAsync(
                stored.Id,
                nowUtc,
                refreshTokenGenerator.Hash(response.RefreshToken),
                cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(RefreshAccessTokenCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
