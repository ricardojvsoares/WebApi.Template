using Application.Abstractions.Data;
using Dapper;
using Domain.Users.Entities;
using Domain.Users.Repositories;

namespace Persistence.Users.Repositories;

internal sealed class UserRepository(
    INpgsqlConnectionFactory connectionFactory)
    : IUserRepository
{
    private const string SelectColumns = """
        id, email, email_normalized, password_hash, display_name, is_active,
        created_at_utc, updated_at_utc
        """;

    private readonly INpgsqlConnectionFactory _connectionFactory = connectionFactory;

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM users
            WHERE id = @Id;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM users
            WHERE email_normalized = @EmailNormalized;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(
                sql,
                new { EmailNormalized = User.NormalizeEmail(email) },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM users
                WHERE email_normalized = @EmailNormalized);
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                sql,
                new { EmailNormalized = User.NormalizeEmail(email) },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<User>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM users
            ORDER BY created_at_utc DESC, id
            LIMIT @Take OFFSET @Skip;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var users = await connection.QueryAsync<User>(
            new CommandDefinition(
                sql,
                new { Skip = skip, Take = take },
                cancellationToken: cancellationToken));

        return [.. users];
    }

    public async Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(*) FROM users;";

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                cancellationToken: cancellationToken));
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO users (
                id, email, email_normalized, password_hash, display_name, is_active,
                created_at_utc, updated_at_utc)
            VALUES (
                @Id, @Email, @EmailNormalized, @PasswordHash, @DisplayName, @IsActive,
                @CreatedAtUtc, @UpdatedAtUtc);
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                user,
                cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE users
            SET email = @Email,
                email_normalized = @EmailNormalized,
                password_hash = @PasswordHash,
                display_name = @DisplayName,
                is_active = @IsActive,
                updated_at_utc = @UpdatedAtUtc
            WHERE id = @Id;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                user,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM users
            WHERE id = @Id;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken));

        return affected > 0;
    }

    public async Task<IReadOnlyList<string>> GetPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT DISTINCT p.name
            FROM user_roles ur
            INNER JOIN role_permissions rp ON rp.role_id = ur.role_id
            INNER JOIN permissions p ON p.id = rp.permission_id
            WHERE ur.user_id = @UserId
            ORDER BY p.name;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var permissions = await connection.QueryAsync<string>(
            new CommandDefinition(
                sql,
                new { UserId = userId },
                cancellationToken: cancellationToken));

        return [.. permissions];
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT r.name
            FROM user_roles ur
            INNER JOIN roles r ON r.id = ur.role_id
            WHERE ur.user_id = @UserId
            ORDER BY r.name;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var roles = await connection.QueryAsync<string>(
            new CommandDefinition(
                sql,
                new { UserId = userId },
                cancellationToken: cancellationToken));

        return [.. roles];
    }
}
