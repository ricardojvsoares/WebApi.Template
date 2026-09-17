using Application.Abstractions.Data;
using Dapper;
using Domain.Users.Entities;
using Domain.Users.Repositories;
using Persistence.Users.Sql;

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
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<RefreshToken>(
            new CommandDefinition(
                RefreshTokenSql.GetByTokenHash,
                new { TokenHash = tokenHash },
                cancellationToken: cancellationToken));
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                RefreshTokenSql.Insert,
                refreshToken,
                cancellationToken: cancellationToken));
    }

    public async Task RevokeAsync(
        Guid id,
        DateTime revokedAtUtc,
        string? replacedByTokenHash,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                RefreshTokenSql.Revoke,
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
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteAsync(
            new CommandDefinition(
                RefreshTokenSql.RevokeAllForUser,
                new { UserId = userId, RevokedAtUtc = revokedAtUtc },
                cancellationToken: cancellationToken));
    }
}
