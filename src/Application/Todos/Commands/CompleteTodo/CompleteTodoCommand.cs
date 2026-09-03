namespace Application.Todos.Commands.CompleteTodo;

public sealed record CompleteTodoCommand(
    Guid Id,
    bool IsCompleted);
