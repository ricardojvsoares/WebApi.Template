using Application.Abstractions.Authentication;
using Domain.Products.Entities;
using Domain.Products.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.CreateProduct;

public static class CreateProductCommandHandler
{
    public static async Task<ErrorOr<CreateProductResponse>> HandleAsync(
        CreateProductCommand command,
        ILogger logger,
        IProductRepository productRepository,
        ICurrentUser currentUser,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (currentUser.UserId is not Guid createdBy)
            {
                return Error.Unauthorized(description: "The request is not authenticated.");
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
            var image = new Uri(command.Image.Trim(), UriKind.Absolute);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = command.Name.Trim(),
                Description = command.Description?.Trim(),
                Price = command.Price,
                Image = image,
                CreatedBy = createdBy,
                CreatedAtUtc = nowUtc,
                UpdatedBy = createdBy,
                UpdatedAtUtc = nowUtc
            };

            await productRepository.AddAsync(
                product,
                cancellationToken);

            return new CreateProductResponse(
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
                nameof(CreateProductCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
