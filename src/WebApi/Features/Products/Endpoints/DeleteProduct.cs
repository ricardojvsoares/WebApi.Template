using Application.Products.Commands.DeleteProduct;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Products.Endpoints;

internal sealed class DeleteProduct
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", HandleAsync)
            .HasApiVersion(1)
            .WithName("DeleteProduct")
            .WithSummary("Delete product")
            .RequirePermission(ProductPermissions.Delete)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        DeleteProductCommand command = new(id);

        var result = await bus.InvokeAsync<ErrorOr<Deleted>>(
            command,
            cancellationToken);

        return result.ToNoContent();
    }
}
