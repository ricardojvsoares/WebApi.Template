namespace Domain.Authorization;

/// <summary>
/// Aggregates every permission group so the EF seed data and the startup consistency
/// check can enumerate all permissions without knowing the individual groups.
/// Adding a feature means adding one line here and one new permissions file.
/// </summary>
public static class PermissionRegistry
{
    public static IReadOnlyList<string> All { get; } =
    [
        .. TodoPermissions.All,
        .. ProductPermissions.All,
        .. UserPermissions.All
    ];
}
