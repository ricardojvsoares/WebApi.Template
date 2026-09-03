using Application.Authentication;
using Application.Authentication.Commands.Login;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Auth.Endpoints;

internal sealed class Login
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPost("/login", HandleAsync)
            .HasApiVersion(1)
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Log in")
            .Produces<AuthenticationResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        LoginCommand command,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<ErrorOr<AuthenticationResponse>>(
            command,
            cancellationToken);

        return result.ToOk();
    }
}
