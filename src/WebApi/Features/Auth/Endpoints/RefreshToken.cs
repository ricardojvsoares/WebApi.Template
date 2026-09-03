using Application.Authentication;
using Application.Authentication.Commands.RefreshAccessToken;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Auth.Endpoints;

/// <summary>
/// Rotates the refresh token: the presented token is revoked and linked to its
/// replacement, so replaying an old token is detectable.
/// </summary>
internal sealed class RefreshToken
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPost("/refresh", HandleAsync)
            .HasApiVersion(1)
            .AllowAnonymous()
            .WithName("RefreshToken")
            .WithSummary("Refresh tokens")
            .Produces<AuthenticationResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        RefreshAccessTokenCommand command,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<ErrorOr<AuthenticationResponse>>(
            command,
            cancellationToken);

        return result.ToOk();
    }
}
