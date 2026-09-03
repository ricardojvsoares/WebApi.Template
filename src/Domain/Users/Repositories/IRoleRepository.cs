using Domain.Users.Entities;

namespace Domain.Users.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Role>> ListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Grants a role to a user. Repeated calls for the same pair are a no-op.
    /// </summary>
    Task AssignToUserAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveFromUserAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Names of every permission that has a row in the permissions table. Used at startup
    /// to check the seeded data still matches the permission constants in code.
    /// </summary>
    Task<IReadOnlyList<string>> GetKnownPermissionNamesAsync(
        CancellationToken cancellationToken = default);
}
