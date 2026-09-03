using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;
using Persistence.Migrations;
using Persistence.Seeding;

namespace Persistence;

public static class DatabaseStartup
{
    /// <summary>
    /// Brings the database up to date and verifies it before the API serves traffic:
    /// applies pending migrations when configured, seeds the bootstrap administrator, and
    /// checks the seeded permissions still match the constants in code.
    /// </summary>
    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = services.GetRequiredService<PostgresOptions>();

        if (options.RunMigrationsOnStartup)
        {
            DatabaseMigrator.ApplyPending(services);
        }

        using var scope = services.CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<AdminUserSeeder>()
            .SeedAsync(cancellationToken);

        await scope.ServiceProvider
            .GetRequiredService<PermissionConsistencyCheck>()
            .VerifyAsync(cancellationToken);
    }
}
