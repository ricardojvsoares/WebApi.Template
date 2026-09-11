namespace Application.Todos.Queries.GetTodoById;

public sealed record GetTodoByIdResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDateUtc,
    DateTime? CompletedAtUtc,
    Guid OwnerUserId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
