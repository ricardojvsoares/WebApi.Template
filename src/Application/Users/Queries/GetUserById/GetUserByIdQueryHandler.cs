using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetUserById;

public static class GetUserByIdQueryHandler
{
    public static async Task<ErrorOr<UserResponse>> HandleAsync(
        GetUserByIdQuery query,
        ILogger logger,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(
                query.Id,
                cancellationToken);

            if (user is null)
            {
                return Error.NotFound(description: "The user was not found.");
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
                nameof(GetUserByIdQueryHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
