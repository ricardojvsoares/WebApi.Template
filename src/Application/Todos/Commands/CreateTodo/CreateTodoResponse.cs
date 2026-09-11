namespace Application.Todos.Commands.CreateTodo;

public sealed record CreateTodoResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDateUtc,
    DateTime? CompletedAtUtc,
    Guid OwnerUserId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
