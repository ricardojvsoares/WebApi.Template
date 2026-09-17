using System.Globalization;
using Domain.Authorization;
using Domain.Users.Entities;
using AuthRoles = Domain.Authorization.Roles;

namespace Persistence.Seeding;

/// <summary>
/// Stable identifiers and rows for roles/permissions so fresh databases match across environments.
/// </summary>
internal static class ReferenceData
{
    public static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid UserRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly IReadOnlyDictionary<string, Guid> PermissionIds =
        PermissionRegistry.All.ToDictionary(
            permission => permission,
            permission => CreateDeterministicGuid(permission),
            StringComparer.Ordinal);

    public static IReadOnlyList<Permission> Permissions { get; } =
    [
        .. PermissionRegistry.All.Select(name => new Permission
        {
            Id = PermissionIds[name],
            Name = name,
            Description = Describe(name)
        })
    ];

    public static IReadOnlyList<Role> Roles { get; } =
    [
        new Role
        {
            Id = AdminRoleId,
            Name = AuthRoles.Admin,
            Description = "Every permission the API defines."
        },
        new Role
        {
            Id = UserRoleId,
            Name = AuthRoles.User,
            Description = "Baseline access: full control over the caller's own todos."
        }
    ];

    public static IReadOnlyList<(Guid RoleId, Guid PermissionId)> RolePermissions { get; } =
    [
        .. PermissionIds.Values.Select(permissionId => (AdminRoleId, permissionId)),
        .. TodoPermissions.All.Concat(ProductPermissions.All)
            .Select(permission => (UserRoleId, PermissionIds[permission]))
    ];

    private static Guid CreateDeterministicGuid(
        string value)
    {
        var hash = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes($"permission:{value}"));

        return new Guid(hash.AsSpan(0, 16));
    }

    private static string Describe(
        string permission)
    {
        var separator = permission.IndexOf(':', StringComparison.Ordinal);

        if (separator <= 0)
        {
            return permission;
        }

        var action = permission[..separator];
        var resource = permission[(separator + 1)..];

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{char.ToUpper(action[0], CultureInfo.InvariantCulture)}{action[1..]} {resource}");
    }
}
