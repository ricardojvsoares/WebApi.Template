using Application.Abstractions.Authentication;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UpdateUser;

public static class UpdateUserCommandHandler
{
    public static async Task<ErrorOr<UserResponse>> HandleAsync(
        UpdateUserCommand command,
        ILogger logger,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUser currentUser,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (command.Id == currentUser.UserId && !command.IsActive)
            {
                return Error.Validation(
                    code: nameof(command.IsActive),
                    description: "You cannot deactivate your own account.");
            }

            var user = await userRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            if (user is null)
            {
                return Error.NotFound(description: "The user was not found.");
            }

            var wasActive = user.IsActive;
            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

            user.UpdateProfile(
                command.DisplayName.Trim(),
                command.IsActive,
                nowUtc);

            await userRepository.UpdateAsync(
                user,
                cancellationToken);

            // Deactivating an account must also cut off its ability to mint new access
            // tokens, otherwise the refresh token keeps working until it expires.
            if (wasActive && !user.IsActive)
            {
                await refreshTokenRepository.RevokeAllForUserAsync(
                    user.Id,
                    nowUtc,
                    cancellationToken);
            }

            var roles = await userRepository.GetRoleNamesAsync(
                user.Id,
                cancellationToken);

            return user.ToResponse(roles);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(UpdateUserCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
