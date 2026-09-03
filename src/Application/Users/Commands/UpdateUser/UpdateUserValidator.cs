using FluentValidation;

namespace Application.Users.Commands.UpdateUser;

internal sealed class UpdateUserValidator
    : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty();

        RuleFor(r => r.DisplayName)
            .NotEmpty()
            .MaximumLength(128);
    }
}
