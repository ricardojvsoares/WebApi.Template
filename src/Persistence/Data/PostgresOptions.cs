using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Persistence.Data;

/// <summary>
/// Owns the single Postgres connection string so that both the connection factory
/// and the migration runner build it from the same place.
/// </summary>
internal sealed class PostgresOptions
{
    private const int DefaultPort = 5432;

    private PostgresOptions(
        string connectionString,
        bool runMigrationsOnStartup)
    {
        ConnectionString = connectionString;
        RunMigrationsOnStartup = runMigrationsOnStartup;
    }

    public string ConnectionString { get; }

    public bool RunMigrationsOnStartup { get; }

    public static PostgresOptions FromConfiguration(
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (!int.TryParse(configuration["PG_PORT"], out int port))
        {
            port = DefaultPort;
        }

        NpgsqlConnectionStringBuilder builder = new()
        {
            Host = configuration["PG_HOST"],
            Port = port,
            Database = configuration["PG_DATABASE"],
            Username = configuration["PG_USERNAME"],
            Password = configuration["PG_PASSWORD"]
        };

        if (!bool.TryParse(configuration["PG_RUN_MIGRATIONS_ON_STARTUP"], out bool runMigrations))
        {
            runMigrations = false;
        }

        return new PostgresOptions(
            builder.ConnectionString,
            runMigrations);
    }
}
