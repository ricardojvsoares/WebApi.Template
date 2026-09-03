using FluentValidation;

namespace Application.Users.Commands.CreateUser;

internal sealed class CreateUserValidator
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(12)
            .MaximumLength(256);

        RuleFor(r => r.DisplayName)
            .NotEmpty()
            .MaximumLength(128);

        RuleForEach(r => r.Roles)
            .NotEmpty()
            .MaximumLength(64);
    }
}
