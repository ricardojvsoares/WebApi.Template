using Domain.Products.Entities;
using Domain.Products.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.DeleteProduct;

public static class DeleteProductCommandHandler
{
    public static async Task<ErrorOr<Deleted>> HandleAsync(
        DeleteProductCommand command,
        ILogger logger,
        IProductRepository productRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await productRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            ErrorOr<Product> access = ProductAccess.EnsureExists(
                existing);

            if (access.IsError)
            {
                return access.Errors;
            }

            await productRepository.DeleteAsync(
                access.Value.Id,
                cancellationToken);

            return Result.Deleted;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(DeleteProductCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
