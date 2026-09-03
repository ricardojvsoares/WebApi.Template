using Application.Abstractions.Data;
using Dapper;
using Domain.Users.Entities;
using Domain.Users.Repositories;

namespace Persistence.Users.Repositories;

internal sealed class RoleRepository(
    INpgsqlConnectionFactory connectionFactory)
    : IRoleRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory = connectionFactory;

    public async Task<Role?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, name, description
            FROM roles
            WHERE lower(name) = lower(@Name);
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Role>(
            new CommandDefinition(
                sql,
                new { Name = name },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Role>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, name, description
            FROM roles
            ORDER BY name;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var roles = await connection.QueryAsync<Role>(
            new CommandDefinition(
                sql,
                cancellationToken: cancellationToken));

        return [.. roles];
    }

    public async Task AssignToUserAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO user_roles (user_id, role_id)
            VALUES (@UserId, @RoleId)
            ON CONFLICT (user_id, role_id) DO NOTHING;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { UserId = userId, RoleId = roleId },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> RemoveFromUserAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM user_roles
            WHERE user_id = @UserId
              AND role_id = @RoleId;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { UserId = userId, RoleId = roleId },
                cancellationToken: cancellationToken));

        return affected > 0;
    }

    public async Task<IReadOnlyList<string>> GetKnownPermissionNamesAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT name
            FROM permissions
            ORDER BY name;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var permissions = await connection.QueryAsync<string>(
            new CommandDefinition(
                sql,
                cancellationToken: cancellationToken));

        return [.. permissions];
    }
}
