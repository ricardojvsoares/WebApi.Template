using FluentValidation;

namespace Application.Authentication.Commands.Register;

internal sealed class RegisterValidator
    : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
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
    }
}
