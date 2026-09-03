using Application.Abstractions.Authentication;
using Application.Common;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Todos.Commands.UpdateTodo;

public static class UpdateTodoCommandHandler
{
    public static async Task<ErrorOr<TodoResponse>> HandleAsync(
        UpdateTodoCommand command,
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

            todo.Update(
                command.Title.Trim(),
                command.Description?.Trim(),
                UtcDateTimes.Normalize(command.DueDateUtc),
                timeProvider.GetUtcNow().UtcDateTime);

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
                nameof(UpdateTodoCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
