using System.Diagnostics.CodeAnalysis;
using Application.Abstractions.Authentication;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Authentication.Commands.Logout;

public static class LogoutCommandHandler
{
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Wolverine binds the handler on the message parameter's type, so it is required even though this command carries no data.")]
    public static async Task<ErrorOr<Success>> HandleAsync(
        LogoutCommand command,
        ILogger logger,
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUser currentUser,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (currentUser.UserId is not Guid userId)
            {
                return Error.Unauthorized(description: "The request is not authenticated.");
            }

            var revoked = await refreshTokenRepository.RevokeAllForUserAsync(
                userId,
                timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken);

            logger.LogInformation(
                "Revoked {Count} refresh token(s) for user {UserId}",
                revoked,
                userId);

            return Result.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(LogoutCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
