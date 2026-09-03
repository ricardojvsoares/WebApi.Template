using FluentValidation;

namespace Application.Todos.Commands.CreateTodo;

internal sealed class CreateTodoValidator
    : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(r => r.Description)
            .MaximumLength(2000);
    }
}
