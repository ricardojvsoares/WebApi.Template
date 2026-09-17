using Application.Abstractions.Data;
using Npgsql;

namespace Persistence.Data;

internal sealed class NpgsqlConnectionFactory(
    PostgresOptions options)
    : INpgsqlConnectionFactory
{
    public async Task<NpgsqlConnection> CreateOpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(options.ConnectionString);

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        return connection;
    }
}
