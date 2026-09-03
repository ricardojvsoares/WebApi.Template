using Application.Authentication;
using Application.Authentication.Queries.GetCurrentUser;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Auth.Endpoints;

internal sealed class GetCurrentUser
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapGet("/me", HandleAsync)
            .HasApiVersion(1)
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithSummary("Current user")
            .Produces<CurrentUserResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> HandleAsync(
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<ErrorOr<CurrentUserResponse>>(
            new GetCurrentUserQuery(),
            cancellationToken);

        return result.ToOk();
    }
}
