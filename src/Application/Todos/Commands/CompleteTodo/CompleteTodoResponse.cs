namespace Application.Todos.Commands.CompleteTodo;

public sealed record CompleteTodoResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDateUtc,
    DateTime? CompletedAtUtc,
    Guid OwnerUserId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
