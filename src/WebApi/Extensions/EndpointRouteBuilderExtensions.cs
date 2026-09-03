using WebApi.Abstractions;

namespace WebApi.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps a single endpoint into the feature's route group. Returns the group so calls
    /// chain, letting a module list its endpoints in one expression.
    /// </summary>
    public static RouteGroupBuilder MapEndpoint<TEndpoint>(
        this RouteGroupBuilder group)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(group);

        return group;
    }
}
