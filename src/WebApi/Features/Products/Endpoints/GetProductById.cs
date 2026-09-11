using Application.Products.Queries.GetProductById;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Products.Endpoints;

internal sealed class GetProductById
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", HandleAsync)
            .HasApiVersion(1)
            .WithName("GetProductById")
            .WithSummary("Get product")
            .RequirePermission(ProductPermissions.Read)
            .Produces<GetProductByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        GetProductByIdQuery query = new(id);

        var result = await bus.InvokeAsync<ErrorOr<GetProductByIdResponse>>(
            query,
            cancellationToken);

        return result.ToOk();
    }
}
