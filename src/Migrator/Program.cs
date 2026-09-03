using Microsoft.Extensions.Hosting;
using Persistence;
using Persistence.Migrations;

// Applies database migrations outside the API process. Reads the same PG_* configuration
// the API does, so both always target the same database.
//
//   Migrator up               apply every pending migration
//   Migrator down --steps 2   roll back the last two applied migrations
//   Migrator list             show each migration and whether it has been applied

var command = args.Length > 0
    ? args[0].ToUpperInvariant()
    : "UP";

// Built without passing args so that bare verbs like "up" are not mistaken for
// configuration switches by the command line configuration provider.
var builder = Host.CreateApplicationBuilder();

builder.Services.AddPersistence(builder.Configuration);

using var host = builder.Build();

try
{
    switch (command)
    {
        case "UP":
            DatabaseMigrator.ApplyPending(host.Services);
            await Console.Out.WriteLineAsync("Migrations applied.");
            break;

        case "DOWN":
            var steps = ReadSteps(args);

            if (steps < 1)
            {
                await Console.Error.WriteLineAsync("--steps must be a positive whole number.");
                return 1;
            }

            DatabaseMigrator.Rollback(host.Services, steps);
            await Console.Out.WriteLineAsync($"Rolled back {steps} migration(s).");
            break;

        case "LIST":
            foreach (var migration in DatabaseMigrator.List(host.Services))
            {
                await Console.Out.WriteLineAsync(
                    $"{(migration.IsApplied ? "[applied]" : "[pending]")} {migration.Version}  {migration.Name}");
            }

            break;

        default:
            await Console.Error.WriteLineAsync(
                $"Unknown command '{args[0]}'. Expected 'up', 'down' or 'list'.");
            return 1;
    }
}
catch (Exception ex)
{
    await Console.Error.WriteLineAsync($"Migration failed: {ex.Message}");
    return 1;
}

return 0;

static int ReadSteps(
    string[] args)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (string.Equals(args[i], "--steps", StringComparison.OrdinalIgnoreCase))
        {
            return int.TryParse(args[i + 1], out int steps) ? steps : 0;
        }
    }

    // Rolling back exactly one migration is the common case.
    return 1;
}
