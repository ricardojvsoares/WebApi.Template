using Application.Abstractions.Data;
using Dapper;
using Domain.Users.Entities;
using Domain.Users.Repositories;
using Persistence.Users.Sql;

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
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Role>(
            new CommandDefinition(
                RoleSql.GetByName,
                new { Name = name },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Role>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var roles = await connection.QueryAsync<Role>(
            new CommandDefinition(
                RoleSql.List,
                cancellationToken: cancellationToken));

        return [.. roles];
    }

    public async Task AssignToUserAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                RoleSql.AssignToUser,
                new { UserId = userId, RoleId = roleId },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> RemoveFromUserAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(
                RoleSql.RemoveFromUser,
                new { UserId = userId, RoleId = roleId },
                cancellationToken: cancellationToken));

        return affected > 0;
    }

    public async Task<IReadOnlyList<string>> GetKnownPermissionNamesAsync(
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var permissions = await connection.QueryAsync<string>(
            new CommandDefinition(
                RoleSql.GetKnownPermissionNames,
                cancellationToken: cancellationToken));

        return [.. permissions];
    }
}
