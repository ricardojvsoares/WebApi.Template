using Application.Abstractions.Authentication;
using Domain.Users.Entities;
using Domain.Users.Repositories;

namespace Application.Authentication;

/// <summary>
/// Shared by login, registration and refresh so all three mint tokens identically.
/// </summary>
internal static class TokenIssuer
{
    public static async Task<AuthenticationResponse> IssueAsync(
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
            RefreshToken.Create(
                user.Id,
                refreshTokenGenerator.Hash(refreshToken),
                nowUtc.Add(refreshTokenGenerator.Lifetime),
                nowUtc),
            cancellationToken);

        return new AuthenticationResponse(
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken);
    }
}
