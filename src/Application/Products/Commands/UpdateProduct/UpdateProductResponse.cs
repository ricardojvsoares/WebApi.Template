namespace Application.Products.Commands.UpdateProduct;

public sealed record UpdateProductResponse(
    Guid Id,
    string Name,
    string? Description,
    float Price,
    Uri Image,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime UpdatedAtUtc,
    Guid UpdatedBy);
