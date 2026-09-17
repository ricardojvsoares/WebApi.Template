using Application.Abstractions.Data;
using Dapper;
using Domain.Users.Entities;
using Domain.Users.Repositories;
using Persistence.Users.Sql;

namespace Persistence.Users.Repositories;

internal sealed class UserRepository(
    INpgsqlConnectionFactory connectionFactory)
    : IUserRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory = connectionFactory;

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(
                UserSql.GetById,
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(
                UserSql.GetByEmail,
                new { EmailNormalized = User.NormalizeEmail(email) },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                UserSql.EmailExists,
                new { EmailNormalized = User.NormalizeEmail(email) },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<User>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var users = await connection.QueryAsync<User>(
            new CommandDefinition(
                UserSql.List,
                new { Skip = skip, Take = take },
                cancellationToken: cancellationToken));

        return [.. users];
    }

    public async Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                UserSql.Count,
                cancellationToken: cancellationToken));
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                UserSql.Insert,
                user,
                cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                UserSql.Update,
                user,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(
                UserSql.Delete,
                new { Id = id },
                cancellationToken: cancellationToken));

        return affected > 0;
    }

    public async Task<IReadOnlyList<string>> GetPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var permissions = await connection.QueryAsync<string>(
            new CommandDefinition(
                UserSql.GetPermissions,
                new { UserId = userId },
                cancellationToken: cancellationToken));

        return [.. permissions];
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var roles = await connection.QueryAsync<string>(
            new CommandDefinition(
                UserSql.GetRoleNames,
                new { UserId = userId },
                cancellationToken: cancellationToken));

        return [.. roles];
    }
}
