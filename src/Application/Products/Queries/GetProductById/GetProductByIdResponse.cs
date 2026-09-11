namespace Application.Products.Queries.GetProductById;

public sealed record GetProductByIdResponse(
    Guid Id,
    string Name,
    string? Description,
    float Price,
    Uri Image,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime UpdatedAtUtc,
    Guid UpdatedBy);
