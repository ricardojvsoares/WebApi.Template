using Domain.Authorization;
using Domain.Users.Repositories;
using Microsoft.Extensions.Logging;

namespace Persistence.Seeding;

/// <summary>
/// Fails startup when a permission constant has no row in the permissions table. Without
/// this the mismatch only shows up as an unexplained 403 at request time, because a
/// permission nobody can be granted is indistinguishable from one nobody has.
/// </summary>
internal sealed class PermissionConsistencyCheck(
    IRoleRepository roleRepository,
    ILogger<PermissionConsistencyCheck> logger)
{
    public async Task VerifyAsync(
        CancellationToken cancellationToken = default)
    {
        var known = await roleRepository.GetKnownPermissionNamesAsync(
            cancellationToken);

        var missing = PermissionRegistry.All
            .Except(known, StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        if (missing.Length > 0)
        {
            throw new InvalidOperationException(
                $"The permissions table is missing {missing.Length} permission(s) declared in code: " +
                $"{string.Join(", ", missing)}. Add a migration that seeds them.");
        }

        // The reverse direction is only worth a warning: a leftover row is harmless, and
        // rolling out a migration ahead of the code that uses it is a legitimate order.
        var orphaned = known
            .Except(PermissionRegistry.All, StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        if (orphaned.Length > 0)
        {
            logger.LogWarning(
                "The permissions table contains {Count} permission(s) that no longer exist in code: {Orphaned}",
                orphaned.Length,
                string.Join(", ", orphaned));
        }
    }
}
