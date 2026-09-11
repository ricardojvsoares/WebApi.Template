namespace Application.Todos.Commands.UpdateTodo;

public sealed record UpdateTodoResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDateUtc,
    DateTime? CompletedAtUtc,
    Guid OwnerUserId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
