using Application.Authentication.Commands.Logout;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Auth.Endpoints;

internal sealed class Logout
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPost("/logout", HandleAsync)
            .HasApiVersion(1)
            .RequireAuthorization()
            .WithName("Logout")
            .WithSummary("Log out")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> HandleAsync(
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<ErrorOr<Success>>(
            new LogoutCommand(),
            cancellationToken);

        return result.ToNoContent();
    }
}
