namespace Application.Products.Commands.CreateProduct;

public sealed record CreateProductResponse(
    Guid Id,
    string Name,
    string? Description,
    float Price,
    Uri Image,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime UpdatedAtUtc,
    Guid UpdatedBy);
