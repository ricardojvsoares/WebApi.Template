using Application.Common;
using Application.Products.Queries.ListProducts;
using Domain.Authorization;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Products.Endpoints;

internal sealed class ListProducts
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
            .HasApiVersion(1)
            .WithName("ListProducts")
            .WithSummary("List products")
            .RequirePermission(ProductPermissions.Read)
            .Produces<ListProductsResponse>()
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken,
        [FromQuery] int page = PageRequest.DefaultPage,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize)
    {
        ListProductsQuery query = new(
            page,
            pageSize);

        var result = await bus.InvokeAsync<ErrorOr<ListProductsResponse>>(
            query,
            cancellationToken);

        return result.ToOk();
    }
}
