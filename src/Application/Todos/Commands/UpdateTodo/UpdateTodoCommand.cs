namespace Application.Todos.Commands.UpdateTodo;

public sealed record UpdateTodoCommand(
    Guid Id,
    string Title,
    string? Description,
    DateTime? DueDateUtc);
