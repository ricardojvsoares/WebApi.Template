using Application.Users.Commands.DeleteUser;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Users.Endpoints;

internal sealed class DeleteUser
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", HandleAsync)
            .HasApiVersion(1)
            .WithName("DeleteUser")
            .WithSummary("Delete user")
            .RequirePermission(UserPermissions.Delete)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        DeleteUserCommand command = new(id);

        var result = await bus.InvokeAsync<ErrorOr<Deleted>>(
            command,
            cancellationToken);

        return result.ToNoContent();
    }
}
