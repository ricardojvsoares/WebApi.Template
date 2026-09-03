using System.Data;
using Application.Abstractions.Data;
using Npgsql;

namespace Persistence.Data;

internal sealed class NpgsqlConnectionFactory(
    PostgresOptions options)
    : INpgsqlConnectionFactory
{
    private readonly PostgresOptions _options = options;

    public async Task<NpgsqlConnection> CreateOpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(
            _options.ConnectionString);

        if (connection.State == ConnectionState.Closed)
        {
            await connection.OpenAsync(
                cancellationToken);
        }

        return connection;
    }
}
