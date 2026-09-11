using FluentValidation;

namespace Application.Products.Commands.UpdateProduct;

internal sealed class UpdateProductValidator
    : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty();

        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(r => r.Description)
            .MaximumLength(2000);

        RuleFor(r => r.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(r => r.Image)
            .NotEmpty()
            .Must(BeAbsoluteUri)
            .WithMessage("Image must be an absolute URI.");
    }

    private static bool BeAbsoluteUri(
        string image)
    {
        return Uri.TryCreate(image, UriKind.Absolute, out _);
    }
}
