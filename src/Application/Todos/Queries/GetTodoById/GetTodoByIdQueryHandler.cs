using Application.Abstractions.Authentication;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Todos.Queries.GetTodoById;

public static class GetTodoByIdQueryHandler
{
    public static async Task<ErrorOr<TodoResponse>> HandleAsync(
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

            return access.Value.ToResponse();
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
