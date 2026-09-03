using Application.Users;
using Application.Users.Queries.GetUserById;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Users.Endpoints;

internal sealed class GetUserById
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", HandleAsync)
            .HasApiVersion(1)
            .WithName("GetUserById")
            .WithSummary("Get user")
            .RequirePermission(UserPermissions.Read)
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        GetUserByIdQuery query = new(id);

        var result = await bus.InvokeAsync<ErrorOr<UserResponse>>(
            query,
            cancellationToken);

        return result.ToOk();
    }
}
