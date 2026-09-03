using Asp.Versioning;
using Carter;
using WebApi.Extensions;
using WebApi.Features.Users.Endpoints;

namespace WebApi.Features.Users;

public sealed class UsersModule
    : ICarterModule
{
    private const string ModuleName = "Users";
    private const string RoutePrefix = "api/v{apiVersion:apiVersion}/users";

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
            .MapEndpoint<CreateUser>()
            .MapEndpoint<ListUsers>()
            .MapEndpoint<GetUserById>()
            .MapEndpoint<UpdateUser>()
            .MapEndpoint<DeleteUser>()
            .MapEndpoint<AssignRoleToUser>();
    }
}
