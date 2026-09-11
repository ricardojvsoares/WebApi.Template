using Application.Abstractions.Authentication;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Todos.Queries.GetTodoById;

public static class GetTodoByIdQueryHandler
{
    public static async Task<ErrorOr<GetTodoByIdResponse>> HandleAsync(
        GetTodoByIdQuery query,
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
                query.Id,
                cancellationToken);

            ErrorOr<Todo> access = TodoAccess.ForOwner(
                existing,
                ownerUserId);

            if (access.IsError)
            {
                return access.Errors;
            }

            var todo = access.Value;

            return new GetTodoByIdResponse(
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
                nameof(GetTodoByIdQueryHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
