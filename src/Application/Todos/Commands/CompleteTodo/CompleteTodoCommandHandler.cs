using Application.Abstractions.Authentication;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Todos.Commands.CompleteTodo;

public static class CompleteTodoCommandHandler
{
    public static async Task<ErrorOr<CompleteTodoResponse>> HandleAsync(
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

            if (command.IsCompleted)
            {
                if (todo.IsCompleted)
                {
                    return Error.Conflict(description: "The todo is already completed.");
                }

                todo.IsCompleted = true;
                todo.CompletedAtUtc = nowUtc;
                todo.UpdatedBy = ownerUserId;
                todo.UpdatedAtUtc = nowUtc;
            }
            else
            {
                if (!todo.IsCompleted)
                {
                    return Error.Conflict(description: "The todo is not completed.");
                }

                todo.IsCompleted = false;
                todo.CompletedAtUtc = null;
                todo.UpdatedBy = ownerUserId;
                todo.UpdatedAtUtc = nowUtc;
            }

            await todoRepository.UpdateAsync(
                todo,
                cancellationToken);

            return new CompleteTodoResponse(
                todo.Id,
                todo.Title,
                todo.Description,
                todo.IsCompleted,
                todo.DueDateUtc,
                todo.CompletedAtUtc,
                todo.OwnerUserId,
                todo.CreatedAtUtc,
                todo.UpdatedAtUtc);
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
