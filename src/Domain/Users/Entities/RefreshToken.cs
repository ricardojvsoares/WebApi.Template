namespace Domain.Users.Entities;

/// <summary>
/// A refresh token as stored. Only the SHA-256 hash of the token is persisted, so a
/// database leak does not hand out usable tokens.
/// </summary>
public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
