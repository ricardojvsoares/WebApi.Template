using Application.Users;
using Application.Users.Commands.AssignRoleToUser;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Users.Endpoints;

/// <summary>
/// Granting a role is how a user gains permissions, so it is guarded by
/// <c>update:user</c> rather than by a permission of its own.
/// </summary>
internal sealed class AssignRoleToUser
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/roles", HandleAsync)
            .HasApiVersion(1)
            .WithName("AssignRoleToUser")
            .WithSummary("Assign role")
            .RequirePermission(UserPermissions.Update)
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        AssignRoleToUserBody body,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        AssignRoleToUserCommand command = new(
            id,
            body.RoleName);

        var result = await bus.InvokeAsync<ErrorOr<UserResponse>>(
            command,
            cancellationToken);

        return result.ToOk();
    }
}

internal sealed record AssignRoleToUserBody(
    string RoleName);
