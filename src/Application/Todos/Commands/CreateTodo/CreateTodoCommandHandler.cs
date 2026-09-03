using Application.Abstractions.Authentication;
using Application.Common;
using Application.Todos.Events;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Application.Todos.Commands.CreateTodo;

public static class CreateTodoCommandHandler
{
    public static async Task<ErrorOr<TodoResponse>> HandleAsync(
        CreateTodoCommand command,
        ILogger logger,
        ITodoRepository todoRepository,
        ICurrentUser currentUser,
        TimeProvider timeProvider,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (currentUser.UserId is not Guid ownerUserId)
            {
                return Error.Unauthorized(description: "The request is not authenticated.");
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

            var todo = Todo.Create(
                command.Title.Trim(),
                command.Description?.Trim(),
                UtcDateTimes.Normalize(command.DueDateUtc),
                ownerUserId,
                nowUtc);

            await todoRepository.AddAsync(
                todo,
                cancellationToken);

            await bus.PublishAsync(new TodoCreatedEvent(
                todo.Id,
                todo.Title,
                todo.OwnerUserId,
                todo.CreatedAtUtc));

            return todo.ToResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(CreateTodoCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
