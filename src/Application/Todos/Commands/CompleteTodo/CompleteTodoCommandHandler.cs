using Application.Abstractions.Authentication;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Todos.Commands.CompleteTodo;

public static class CompleteTodoCommandHandler
{
    public static async Task<ErrorOr<TodoResponse>> HandleAsync(
        CompleteTodoCommand command,
        ILogger logger,
        ITodoRepository todoRepository,
        ICurrentUser currentUser,
        TimeProvider timeProvider,
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

            var todo = access.Value;
            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

            var changed = command.IsCompleted
                ? todo.Complete(nowUtc)
                : todo.Reopen(nowUtc);

            if (!changed)
            {
                return Error.Conflict(description: command.IsCompleted
                    ? "The todo is already completed."
                    : "The todo is not completed.");
            }

            await todoRepository.UpdateAsync(
                todo,
                cancellationToken);

            return todo.ToResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(CompleteTodoCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
