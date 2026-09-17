using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;

namespace Persistence.Migrations;

/// <summary>
/// Thin wrapper over EF Core migrations.
/// </summary>
public static class DatabaseMigrator
{
    public static void ApplyPending(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }

    /// <summary>
    /// Rolls back the given number of applied migrations, newest first.
    /// </summary>
    public static void Rollback(
        IServiceProvider services,
        int steps)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var applied = dbContext.Database.GetAppliedMigrations().ToList();

        if (steps > applied.Count)
        {
            throw new InvalidOperationException(
                $"Cannot roll back {steps} migration(s); only {applied.Count} applied.");
        }

        var targetIndex = applied.Count - steps - 1;
        var target = targetIndex >= 0
            ? applied[targetIndex]
            : Migration.InitialDatabase;

        var migrator = dbContext.GetService<IMigrator>();
        migrator.Migrate(target);
    }

    /// <summary>
    /// Every discovered migration with its applied state, oldest first.
    /// </summary>
    public static IReadOnlyList<MigrationStatus> List(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var applied = dbContext.Database.GetAppliedMigrations().ToHashSet(StringComparer.Ordinal);
        var all = dbContext.Database.GetMigrations();

        return
        [
            .. all.Select(name => new MigrationStatus(
                name,
                name,
                applied.Contains(name)))
        ];
    }
}
