namespace Application.Todos.Commands.CreateTodo;

public sealed record CreateTodoCommand(
    string Title,
    string? Description,
    DateTime? DueDateUtc);
