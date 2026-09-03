using FluentValidation;

namespace Application.Users.Commands.AssignRoleToUser;

internal sealed class AssignRoleToUserValidator
    : AbstractValidator<AssignRoleToUserCommand>
{
    public AssignRoleToUserValidator()
    {
        RuleFor(r => r.UserId)
            .NotEmpty();

        RuleFor(r => r.RoleName)
            .NotEmpty()
            .MaximumLength(64);
    }
}
