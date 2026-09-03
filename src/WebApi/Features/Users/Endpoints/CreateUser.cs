using Application.Users;
using Application.Users.Commands.CreateUser;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Users.Endpoints;

internal sealed class CreateUser
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
            .HasApiVersion(1)
            .WithName("CreateUser")
            .WithSummary("Create user")
            .RequirePermission(UserPermissions.Create)
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        CreateUserCommand command,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<ErrorOr<UserResponse>>(
            command,
            cancellationToken);

        return result.ToCreated(user => $"/api/v1/users/{user.Id}");
    }
}
