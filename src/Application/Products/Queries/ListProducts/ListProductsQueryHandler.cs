using Application.Common;
using Domain.Products.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Products.Queries.ListProducts;

public static class ListProductsQueryHandler
{
    public static async Task<ErrorOr<ListProductsResponse>> HandleAsync(
        ListProductsQuery query,
        ILogger logger,
        IProductRepository productRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var totalCount = await productRepository.CountAsync(
                cancellationToken);

            var products = await productRepository.ListAsync(
                PageRequest.ToSkip(query.Page, query.PageSize),
                query.PageSize,
                cancellationToken);

            return new ListProductsResponse(
                [.. products.Select(product => new ListProductsItem(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.Image,
                    product.CreatedAtUtc,
                    product.CreatedBy,
                    product.UpdatedAtUtc,
                    product.UpdatedBy))],
                query.Page,
                query.PageSize,
                totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(ListProductsQueryHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
