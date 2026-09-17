using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Persistence.Data;

/// <summary>
/// Owns the single Postgres connection string so that DbContext and the migration runner
/// build it from the same place.
/// </summary>
internal sealed class PostgresOptions
{
    private const int DefaultPort = 5432;

    private PostgresOptions(
        string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

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

        return new PostgresOptions(
            builder.ConnectionString
        );
    }
}
