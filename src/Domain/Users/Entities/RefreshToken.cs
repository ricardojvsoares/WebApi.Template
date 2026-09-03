namespace Domain.Users.Entities;

/// <summary>
/// A refresh token as stored. Only the SHA-256 hash of the token is persisted, so a
/// database leak does not hand out usable tokens.
/// </summary>
public sealed class RefreshToken
{
    private RefreshToken() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public bool IsActive(
        DateTime nowUtc)
    {
        return RevokedAtUtc is null && ExpiresAtUtc > nowUtc;
    }

    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc,
        DateTime nowUtc)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAtUtc = expiresAtUtc,
            RevokedAtUtc = null,
            ReplacedByTokenHash = null,
            CreatedAtUtc = nowUtc
        };
    }
}
