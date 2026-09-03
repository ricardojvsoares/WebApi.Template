using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization;

internal sealed class PermissionRequirement(
    string permission)
    : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
