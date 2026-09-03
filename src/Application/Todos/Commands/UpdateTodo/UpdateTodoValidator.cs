using FluentValidation;

namespace Application.Todos.Commands.UpdateTodo;

internal sealed class UpdateTodoValidator
    : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty();

        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(r => r.Description)
            .MaximumLength(2000);
    }
}
