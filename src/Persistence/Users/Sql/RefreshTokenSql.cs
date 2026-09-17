namespace Persistence.Users.Sql;

internal static class RefreshTokenSql
{
    private const string Columns = """
        id, user_id, token_hash, expires_at_utc, revoked_at_utc,
        replaced_by_token_hash, created_at_utc
        """;

    public const string GetByTokenHash = $"""
        SELECT {Columns}
        FROM refresh_tokens
        WHERE token_hash = @TokenHash;
        """;

    public const string Insert = """
        INSERT INTO refresh_tokens (
            id, user_id, token_hash, expires_at_utc, revoked_at_utc,
            replaced_by_token_hash, created_at_utc)
        VALUES (
            @Id, @UserId, @TokenHash, @ExpiresAtUtc, @RevokedAtUtc,
            @ReplacedByTokenHash, @CreatedAtUtc);
        """;

    public const string Revoke = """
        UPDATE refresh_tokens
        SET revoked_at_utc = @RevokedAtUtc,
            replaced_by_token_hash = @ReplacedByTokenHash
        WHERE id = @Id
          AND revoked_at_utc IS NULL;
        """;

    public const string RevokeAllForUser = """
        UPDATE refresh_tokens
        SET revoked_at_utc = @RevokedAtUtc
        WHERE user_id = @UserId
          AND revoked_at_utc IS NULL;
        """;
}
