using Domain.Products.Entities;
using ErrorOr;

namespace Application.Products;

/// <summary>
/// Products are a shared catalog. A permission grants the right to perform an action;
/// there is no per-instance ownership check beyond existence.
/// </summary>
internal static class ProductAccess
{
    public static ErrorOr<Product> EnsureExists(
        Product? product)
    {
        return product ?? (ErrorOr<Product>)Error.NotFound(description: "The product was not found.");
    }
}
