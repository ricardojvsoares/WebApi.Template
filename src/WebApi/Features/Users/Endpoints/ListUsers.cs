using Application.Common;
using Application.Users;
using Application.Users.Queries.ListUsers;
using Domain.Authorization;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Users.Endpoints;

internal sealed class ListUsers
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
            .HasApiVersion(1)
            .WithName("ListUsers")
            .WithSummary("List users")
            .RequirePermission(UserPermissions.Read)
            .Produces<PagedResponse<UserResponse>>()
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken,
        [FromQuery] int page = PageRequest.DefaultPage,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize)
    {
        ListUsersQuery query = new(
            page,
            pageSize);

        var result = await bus.InvokeAsync<ErrorOr<PagedResponse<UserResponse>>>(
            query,
            cancellationToken);

        return result.ToOk();
    }
}
