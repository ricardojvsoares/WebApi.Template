namespace Domain.Authorization;

/// <summary>
/// Permissions guarding the Users feature. Every entry must have a matching row in the
/// permissions table, which is seeded from <see cref="PermissionRegistry.All" />.
/// </summary>
public static class UserPermissions
{
    public const string Create = "create:user";
    public const string Read = "read:user";
    public const string Update = "update:user";
    public const string Delete = "delete:user";

    public static IReadOnlyList<string> All { get; } =
    [
        Create,
        Read,
        Update,
        Delete
    ];
}
