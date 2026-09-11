using Application.Common;
using FluentValidation;

namespace Application.Products.Queries.ListProducts;

internal sealed class ListProductsValidator
    : AbstractValidator<ListProductsQuery>
{
    public ListProductsValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(PageRequest.MaxPageSize);
    }
}
