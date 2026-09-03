using Application.Abstractions.Authentication;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.DeleteUser;

public static class DeleteUserCommandHandler
{
    public static async Task<ErrorOr<Deleted>> HandleAsync(
        DeleteUserCommand command,
        ILogger logger,
        IUserRepository userRepository,
        ICurrentUser currentUser,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Without this, the last administrator can lock everyone out of the API.
            if (command.Id == currentUser.UserId)
            {
                return Error.Validation(
                    code: nameof(command.Id),
                    description: "You cannot delete your own account.");
            }

            var deleted = await userRepository.DeleteAsync(
                command.Id,
                cancellationToken);

            if (!deleted)
            {
                return Error.NotFound(description: "The user was not found.");
            }

            return Result.Deleted;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(DeleteUserCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
