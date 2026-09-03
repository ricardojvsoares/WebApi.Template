namespace WebApi.Extensions;

internal static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Requires a single <c>action:resource</c> permission. The policy is created on
    /// demand by <see cref="Authorization.PermissionPolicyProvider" />, so the permission
    /// name doubles as the policy name and needs no registration.
    /// </summary>
    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder builder,
        string permission)
    {
        return builder
            .RequireAuthorization(permission)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
