using Domain.Products.Entities;
using Domain.Products.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Products.Queries.GetProductById;

public static class GetProductByIdQueryHandler
{
    public static async Task<ErrorOr<GetProductByIdResponse>> HandleAsync(
        GetProductByIdQuery query,
        ILogger logger,
        IProductRepository productRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await productRepository.GetByIdAsync(
                query.Id,
                cancellationToken);

            ErrorOr<Product> access = ProductAccess.EnsureExists(
                existing);

            if (access.IsError)
            {
                return access.Errors;
            }

            var product = access.Value;

            return new GetProductByIdResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Image,
                product.CreatedAtUtc,
                product.CreatedBy,
                product.UpdatedAtUtc,
                product.UpdatedBy);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(GetProductByIdQueryHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
