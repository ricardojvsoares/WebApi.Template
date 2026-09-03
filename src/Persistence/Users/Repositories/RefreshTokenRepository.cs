using Application.Abstractions.Data;
using Dapper;
using Domain.Users.Entities;
using Domain.Users.Repositories;

namespace Persistence.Users.Repositories;

internal sealed class RefreshTokenRepository(
    INpgsqlConnectionFactory connectionFactory)
    : IRefreshTokenRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory = connectionFactory;

    public async Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, user_id, token_hash, expires_at_utc, revoked_at_utc,
                   replaced_by_token_hash, created_at_utc
            FROM refresh_tokens
            WHERE token_hash = @TokenHash;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<RefreshToken>(
            new CommandDefinition(
                sql,
                new { TokenHash = tokenHash },
                cancellationToken: cancellationToken));
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO refresh_tokens (
                id, user_id, token_hash, expires_at_utc, revoked_at_utc,
                replaced_by_token_hash, created_at_utc)
            VALUES (
                @Id, @UserId, @TokenHash, @ExpiresAtUtc, @RevokedAtUtc,
                @ReplacedByTokenHash, @CreatedAtUtc);
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                refreshToken,
                cancellationToken: cancellationToken));
    }

    public async Task RevokeAsync(
        Guid id,
        DateTime revokedAtUtc,
        string? replacedByTokenHash,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE refresh_tokens
            SET revoked_at_utc = @RevokedAtUtc,
                replaced_by_token_hash = @ReplacedByTokenHash
            WHERE id = @Id
              AND revoked_at_utc IS NULL;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Id = id,
                    RevokedAtUtc = revokedAtUtc,
                    ReplacedByTokenHash = replacedByTokenHash
                },
                cancellationToken: cancellationToken));
    }

    public async Task<int> RevokeAllForUserAsync(
        Guid userId,
        DateTime revokedAtUtc,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE refresh_tokens
            SET revoked_at_utc = @RevokedAtUtc
            WHERE user_id = @UserId
              AND revoked_at_utc IS NULL;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { UserId = userId, RevokedAtUtc = revokedAtUtc },
                cancellationToken: cancellationToken));
    }
}
