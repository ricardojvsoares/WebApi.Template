using Application.Users;
using Application.Users.Commands.UpdateUser;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Users.Endpoints;

internal sealed class UpdateUser
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", HandleAsync)
            .HasApiVersion(1)
            .WithName("UpdateUser")
            .WithSummary("Update user")
            .RequirePermission(UserPermissions.Update)
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateUserBody body,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        UpdateUserCommand command = new(
            id,
            body.DisplayName,
            body.IsActive);

        var result = await bus.InvokeAsync<ErrorOr<UserResponse>>(
            command,
            cancellationToken);

        return result.ToOk();
    }
}

internal sealed record UpdateUserBody(
    string DisplayName,
    bool IsActive);
