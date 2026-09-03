using Npgsql;

namespace Application.Abstractions.Data;

public interface INpgsqlConnectionFactory
{
    /// <summary>
    /// Creates a connection that is already open and ready to use.
    /// </summary>
    Task<NpgsqlConnection> CreateOpenConnectionAsync(
        CancellationToken cancellationToken = default);
}
