using Asp.Versioning;
using Carter;
using WebApi.Extensions;
using WebApi.Features.Products.Endpoints;

namespace WebApi.Features.Products;

public sealed class ProductsModule
    : ICarterModule
{
    private const string ModuleName = "Products";
    private const string RoutePrefix = "api/v{apiVersion:apiVersion}/products";

    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .Build();

        var group = app.MapGroup(RoutePrefix)
            .WithApiVersionSet(versionSet)
            .WithTags(ModuleName);

        group
            .MapEndpoint<CreateProduct>()
            .MapEndpoint<ListProducts>()
            .MapEndpoint<GetProductById>()
            .MapEndpoint<UpdateProduct>()
            .MapEndpoint<DeleteProduct>();
    }
}
