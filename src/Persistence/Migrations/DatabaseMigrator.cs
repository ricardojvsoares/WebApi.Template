using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence.Migrations;

/// <summary>
/// Thin wrapper over the FluentMigrator runner. Both the API's startup path and the
/// Migrator console app go through here so they cannot drift apart.
/// </summary>
public static class DatabaseMigrator
{
    public static void ApplyPending(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IMigrationRunner>()
            .MigrateUp();
    }

    /// <summary>
    /// Rolls back the given number of applied migrations, newest first.
    /// </summary>
    public static void Rollback(
        IServiceProvider services,
        int steps)
    {
        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IMigrationRunner>()
            .Rollback(steps);
    }

    /// <summary>
    /// Every discovered migration with its applied state, oldest first.
    /// </summary>
    public static IReadOnlyList<MigrationStatus> List(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        var versionLoader = scope.ServiceProvider.GetRequiredService<IVersionLoader>();

        versionLoader.LoadVersionInfo();
        var applied = versionLoader.VersionInfo;

        return
        [
            .. runner.MigrationLoader
                .LoadMigrations()
                .OrderBy(m => m.Key)
                .Select(m => new MigrationStatus(
                    m.Key,
                    m.Value.Migration.GetType().Name,
                    applied.HasAppliedMigration(m.Key)))
        ];
    }
}
