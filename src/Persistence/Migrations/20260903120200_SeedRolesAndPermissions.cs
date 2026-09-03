using System.Globalization;
using Domain.Authorization;
using FluentMigrator;

namespace Persistence.Migrations;

/// <summary>
/// Seeds the permission rows from <see cref="PermissionRegistry.All" /> so the database
/// mirrors the constants in code, plus the two roles that bundle them.
/// </summary>
[Migration(20260903120200)]
public sealed class SeedRolesAndPermissions
    : Migration
{
    public override void Up()
    {
        // Identifiers are generated here rather than looked up with SQL, so the join rows
        // below need no sub-selects and the migration stays free of hand-built SQL.
        var permissionIds = PermissionRegistry.All.ToDictionary(
            permission => permission,
            _ => Guid.NewGuid(),
            StringComparer.Ordinal);

        var adminRoleId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        foreach (var (name, id) in permissionIds)
        {
            Insert.IntoTable("permissions").Row(new
            {
                id,
                name,
                description = Describe(name)
            });
        }

        Insert.IntoTable("roles").Row(new
        {
            id = adminRoleId,
            name = Roles.Admin,
            description = "Every permission the API defines."
        });

        Insert.IntoTable("roles").Row(new
        {
            id = userRoleId,
            name = Roles.User,
            description = "Baseline access: full control over the caller's own todos."
        });

        foreach (var permissionId in permissionIds.Values)
        {
            Insert.IntoTable("role_permissions").Row(new
            {
                role_id = adminRoleId,
                permission_id = permissionId
            });
        }

        foreach (var permission in TodoPermissions.All)
        {
            Insert.IntoTable("role_permissions").Row(new
            {
                role_id = userRoleId,
                permission_id = permissionIds[permission]
            });
        }
    }

    public override void Down()
    {
        // role_permissions and user_roles rows go with their parents via ON DELETE CASCADE.
        Delete.FromTable("roles").Row(new { name = Roles.Admin });
        Delete.FromTable("roles").Row(new { name = Roles.User });

        foreach (var permission in PermissionRegistry.All)
        {
            Delete.FromTable("permissions").Row(new { name = permission });
        }
    }

    /// <summary>
    /// Turns "create:todo" into "Create todo" for the human-readable description column.
    /// </summary>
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
