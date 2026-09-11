using Application.Common;

namespace Application.Products.Queries.ListProducts;

public sealed record ListProductsQuery(
    int Page = PageRequest.DefaultPage,
    int PageSize = PageRequest.DefaultPageSize);
