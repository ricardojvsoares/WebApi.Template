using Application.Abstractions.Authentication;
using Domain.Products.Entities;
using Domain.Products.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.UpdateProduct;

public static class UpdateProductCommandHandler
{
    public static async Task<ErrorOr<UpdateProductResponse>> HandleAsync(
        UpdateProductCommand command,
        ILogger logger,
        IProductRepository productRepository,
        ICurrentUser currentUser,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (currentUser.UserId is not Guid updatedBy)
            {
                return Error.Unauthorized(description: "The request is not authenticated.");
            }

            var existing = await productRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            ErrorOr<Product> access = ProductAccess.EnsureExists(
                existing);

            if (access.IsError)
            {
                return access.Errors;
            }

            var product = access.Value;
            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

            product.Name = command.Name.Trim();
            product.Description = command.Description?.Trim();
            product.Price = command.Price;
            product.Image = new Uri(command.Image.Trim(), UriKind.Absolute);
            product.UpdatedBy = updatedBy;
            product.UpdatedAtUtc = nowUtc;

            await productRepository.UpdateAsync(
                product,
                cancellationToken);

            return new UpdateProductResponse(
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
                nameof(UpdateProductCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
