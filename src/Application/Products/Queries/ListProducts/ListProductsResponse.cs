namespace Application.Products.Queries.ListProducts;

public sealed record ListProductsItem(
    Guid Id,
    string Name,
    string? Description,
    float Price,
    Uri Image,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime UpdatedAtUtc,
    Guid UpdatedBy);

public sealed record ListProductsResponse(
    IReadOnlyList<ListProductsItem> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
