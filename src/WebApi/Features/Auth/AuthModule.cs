using Asp.Versioning;
using Carter;
using WebApi.Extensions;
using WebApi.Features.Auth.Endpoints;

namespace WebApi.Features.Auth;

public sealed class AuthModule
    : ICarterModule
{
    private const string ModuleName = "Auth";
    private const string RoutePrefix = "api/v{apiVersion:apiVersion}/auth";

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
            .MapEndpoint<Register>()
            .MapEndpoint<Login>()
            .MapEndpoint<RefreshToken>()
            .MapEndpoint<Logout>()
            .MapEndpoint<GetCurrentUser>();
    }
}
