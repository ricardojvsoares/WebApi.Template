using Application.Abstractions.Authentication;
using Application.Common;
using Domain.Todos.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Todos.Queries.ListTodos;

public static class ListTodosQueryHandler
{
    public static async Task<ErrorOr<PagedResponse<TodoResponse>>> HandleAsync(
        ListTodosQuery query,
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

            var totalCount = await todoRepository.CountAsync(
                ownerUserId,
                query.IsCompleted,
                cancellationToken);

            var todos = await todoRepository.ListAsync(
                ownerUserId,
                query.IsCompleted,
                PageRequest.ToSkip(query.Page, query.PageSize),
                query.PageSize,
                cancellationToken);

            return new PagedResponse<TodoResponse>(
                [.. todos.Select(t => t.ToResponse())],
                query.Page,
                query.PageSize,
                totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(ListTodosQueryHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
