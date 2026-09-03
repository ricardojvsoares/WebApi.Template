using Application.Authentication;
using Application.Authentication.Commands.Register;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Auth.Endpoints;

/// <summary>
/// Anonymous self-service signup. The new account receives the baseline role only;
/// elevated roles are granted through the Users feature.
/// </summary>
internal sealed class Register
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPost("/register", HandleAsync)
            .HasApiVersion(1)
            .AllowAnonymous()
            .WithName("Register")
            .WithSummary("Register")
            .Produces<AuthenticationResponse>()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        RegisterCommand command,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<ErrorOr<AuthenticationResponse>>(
            command,
            cancellationToken);

        return result.ToOk();
    }
}
