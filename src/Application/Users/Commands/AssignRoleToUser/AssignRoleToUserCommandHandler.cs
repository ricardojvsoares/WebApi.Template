using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.AssignRoleToUser;

public static class AssignRoleToUserCommandHandler
{
    public static async Task<ErrorOr<UserResponse>> HandleAsync(
        AssignRoleToUserCommand command,
        ILogger logger,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(
                command.UserId,
                cancellationToken);

            if (user is null)
            {
                return Error.NotFound(description: "The user was not found.");
            }

            var role = await roleRepository.GetByNameAsync(
                command.RoleName,
                cancellationToken);

            if (role is null)
            {
                return Error.Validation(
                    code: nameof(command.RoleName),
                    description: $"The role '{command.RoleName}' does not exist.");
            }

            // Idempotent: assigning a role the user already holds is not an error.
            await roleRepository.AssignToUserAsync(
                user.Id,
                role.Id,
                cancellationToken);

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
                nameof(AssignRoleToUserCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
