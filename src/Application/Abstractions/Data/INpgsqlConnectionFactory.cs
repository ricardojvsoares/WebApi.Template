using Npgsql;

namespace Application.Abstractions.Data;

public interface INpgsqlConnectionFactory
{
    Task<NpgsqlConnection> CreateOpenConnectionAsync(
        CancellationToken cancellationToken = default);
}
