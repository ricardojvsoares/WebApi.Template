using Domain.Users.Entities;

namespace Domain.Users.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a token and records which token replaced it, giving a usable audit trail
    /// when a leaked token is replayed.
    /// </summary>
    Task RevokeAsync(
        Guid id,
        DateTime revokedAtUtc,
        string? replacedByTokenHash,
        CancellationToken cancellationToken = default);

    Task<int> RevokeAllForUserAsync(
        Guid userId,
        DateTime revokedAtUtc,
        CancellationToken cancellationToken = default);
}
