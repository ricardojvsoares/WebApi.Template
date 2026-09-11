namespace Domain.Authorization;

/// <summary>
/// Permissions guarding the Products feature. Every entry must have a matching row in the
/// permissions table, which is seeded from <see cref="PermissionRegistry.All" />.
/// </summary>
public static class ProductPermissions
{
    public const string Create = "create:product";
    public const string Read = "read:product";
    public const string Update = "update:product";
    public const string Delete = "delete:product";

    public static IReadOnlyList<string> All { get; } =
    [
        Create,
        Read,
        Update,
        Delete
    ];
}
