using Application.Products.Commands.CreateProduct;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Products.Endpoints;

internal sealed class CreateProduct
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
            .HasApiVersion(1)
            .WithName("CreateProduct")
            .WithSummary("Create product")
            .RequirePermission(ProductPermissions.Create)
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        CreateProductCommand command,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<ErrorOr<CreateProductResponse>>(
            command,
            cancellationToken);

        return result.ToCreated(product => $"/api/v1/products/{product.Id}");
    }
}
