namespace Domain.Authorization;

/// <summary>
/// Permissions guarding the Todos feature. Every entry must have a matching row in the
/// permissions table, which is seeded from <see cref="PermissionRegistry.All" />.
/// </summary>
public static class TodoPermissions
{
    public const string Create = "create:todo";
    public const string Read = "read:todo";
    public const string Update = "update:todo";
    public const string Delete = "delete:todo";

    public static IReadOnlyList<string> All { get; } =
    [
        Create,
        Read,
        Update,
        Delete
    ];
}
