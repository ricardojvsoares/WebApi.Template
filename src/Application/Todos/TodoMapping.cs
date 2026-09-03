using Domain.Todos.Entities;

namespace Application.Todos;

internal static class TodoMapping
{
    public static TodoResponse ToResponse(
        this Todo todo)
    {
        return new TodoResponse(
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
}
