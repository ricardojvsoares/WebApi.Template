using Application.Abstractions.Authentication;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Todos.Commands.DeleteTodo;

public static class DeleteTodoCommandHandler
{
    public static async Task<ErrorOr<Deleted>> HandleAsync(
        DeleteTodoCommand command,
        ILogger logger,
        ITodoRepository todoRepository,
        ICurrentUser currentUser,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (currentUser.UserId is not Guid ownerUserId)
            {
                return Error.Unauthorized(description: "The request is not authenticated.");
            }

            var existing = await todoRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            ErrorOr<Todo> access = TodoAccess.ForOwner(
                existing,
                ownerUserId);

            if (access.IsError)
            {
                return access.Errors;
            }

            await todoRepository.DeleteAsync(
                access.Value.Id,
                cancellationToken);

            return Result.Deleted;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(DeleteTodoCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
