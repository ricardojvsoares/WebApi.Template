using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace WebApi.Authorization;

/// <summary>
/// Builds an authorization policy on demand for any policy name shaped like a permission
/// (<c>action:resource</c>), so adding a permission never means registering a policy.
/// Named policies registered the usual way still take precedence.
/// </summary>
internal sealed class PermissionPolicyProvider(
    IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    private readonly ConcurrentDictionary<string, AuthorizationPolicy> _cache = new(StringComparer.Ordinal);

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        var registered = await base.GetPolicyAsync(policyName);

        if (registered is not null)
        {
            return registered;
        }

        if (!policyName.Contains(':', StringComparison.Ordinal))
        {
            return null;
        }

        return _cache.GetOrAdd(
            policyName,
            static permission => new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build());
    }
}
