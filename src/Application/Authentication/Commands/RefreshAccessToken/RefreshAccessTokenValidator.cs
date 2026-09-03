using FluentValidation;

namespace Application.Authentication.Commands.RefreshAccessToken;

internal sealed class RefreshAccessTokenValidator
    : AbstractValidator<RefreshAccessTokenCommand>
{
    public RefreshAccessTokenValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty();
    }
}
