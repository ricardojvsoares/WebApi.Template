using Application.Products.Commands.UpdateProduct;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Products.Endpoints;

internal sealed class UpdateProduct
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", HandleAsync)
            .HasApiVersion(1)
            .WithName("UpdateProduct")
            .WithSummary("Update product")
            .RequirePermission(ProductPermissions.Update)
            .Produces<UpdateProductResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateProductBody body,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        UpdateProductCommand command = new(
            id,
            body.Name,
            body.Description,
            body.Price,
            body.Image);

        var result = await bus.InvokeAsync<ErrorOr<UpdateProductResponse>>(
            command,
            cancellationToken);

        return result.ToOk();
    }
}

// Route supplies Id; body must not share a generic OpenAPI schema name like Request/Body.
internal sealed record UpdateProductBody(
    string Name,
    string? Description,
    float Price,
    string Image);
