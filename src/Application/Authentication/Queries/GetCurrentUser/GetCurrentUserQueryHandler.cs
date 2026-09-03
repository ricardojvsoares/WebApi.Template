using System.Diagnostics.CodeAnalysis;
using Application.Abstractions.Authentication;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Authentication.Queries.GetCurrentUser;

public static class GetCurrentUserQueryHandler
{
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Wolverine binds the handler on the message parameter's type, so it is required even though this query carries no data.")]
    public static async Task<ErrorOr<CurrentUserResponse>> HandleAsync(
        GetCurrentUserQuery query,
        ILogger logger,
        IUserRepository userRepository,
        ICurrentUser currentUser,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (currentUser.UserId is not Guid userId)
            {
                return Error.Unauthorized(description: "The request is not authenticated.");
            }

            var user = await userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user is null)
            {
                // The token is signed and unexpired but its subject no longer exists.
                return Error.Unauthorized(description: "The request is not authenticated.");
            }

            // Read back from the database rather than from the token claims, so the caller
            // sees permission changes that happened after the token was issued.
            var roles = await userRepository.GetRoleNamesAsync(
                userId,
                cancellationToken);

            var permissions = await userRepository.GetPermissionsAsync(
                userId,
                cancellationToken);

            return new CurrentUserResponse(
                user.Id,
                user.Email,
                user.DisplayName,
                roles,
                permissions);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(GetCurrentUserQueryHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
