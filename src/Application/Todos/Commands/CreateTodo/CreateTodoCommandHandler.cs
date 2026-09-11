using Application.Abstractions.Authentication;
using Application.Common;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Todos.Commands.CreateTodo;

public static class CreateTodoCommandHandler
{
    public static async Task<ErrorOr<CreateTodoResponse>> HandleAsync(
        CreateTodoCommand command,
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

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                Title = command.Title.Trim(),
                Description = command.Description?.Trim(),
                DueDateUtc = UtcDateTimes.Normalize(command.DueDateUtc),
                OwnerUserId = ownerUserId,
                IsCompleted = false,
                CompletedAtUtc = null,
                CreatedAtUtc = nowUtc,
                UpdatedAtUtc = nowUtc
            };

            await todoRepository.AddAsync(
                todo,
                cancellationToken);

            return new CreateTodoResponse(
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
                nameof(CreateTodoCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
