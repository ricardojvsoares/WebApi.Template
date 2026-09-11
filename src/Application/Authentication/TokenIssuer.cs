using Application.Abstractions.Authentication;
using Application.Abstractions.Security;
using Domain.Users.Entities;
using Domain.Users.Repositories;

namespace Application.Authentication;

/// <summary>
/// Shared by login, registration and refresh so all three mint tokens identically.
/// </summary>
internal static class TokenIssuer
{
    public static async Task<TokenIssueResult> IssueAsync(
        User user,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        var permissions = await userRepository.GetPermissionsAsync(
            user.Id,
            cancellationToken);

        var roles = await userRepository.GetRoleNamesAsync(
            user.Id,
            cancellationToken);

        var accessToken = jwtTokenGenerator.CreateAccessToken(
            user,
            permissions,
            roles);

        var refreshToken = refreshTokenGenerator.Create();

        await refreshTokenRepository.AddAsync(
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = refreshTokenGenerator.Hash(refreshToken),
                ExpiresAtUtc = nowUtc.Add(refreshTokenGenerator.Lifetime),
                RevokedAtUtc = null,
                ReplacedByTokenHash = null,
                CreatedAtUtc = nowUtc
            },
            cancellationToken);

        return new TokenIssueResult(
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken);
    }
}
