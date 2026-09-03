using System.Security.Claims;
using System.Text;
using Application.Abstractions.Authentication;
using Domain.Users.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

internal sealed class JwtTokenGenerator(
    IOptions<JwtOptions> options,
    TimeProvider timeProvider)
    : IJwtTokenGenerator
{
    private static readonly JsonWebTokenHandler TokenHandler = new();

    public AccessToken CreateAccessToken(
        User user,
        IReadOnlyList<string> permissions,
        IReadOnlyList<string> roles)
    {
        var settings = options.Value;
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAtUtc = nowUtc.AddMinutes(settings.AccessTokenMinutes);

        List<Claim> claims =
        [
            new(AuthenticationClaims.Subject, user.Id.ToString()),
            new(AuthenticationClaims.Email, user.Email),
            new(AuthenticationClaims.Name, user.DisplayName),
            .. roles.Select(role => new Claim(AuthenticationClaims.Role, role)),
            .. permissions.Select(permission => new Claim(AuthenticationClaims.Permission, permission))
        ];

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(settings.SigningKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = nowUtc,
            NotBefore = nowUtc,
            Expires = expiresAtUtc,
            SigningCredentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256)
        };

        return new AccessToken(
            TokenHandler.CreateToken(descriptor),
            expiresAtUtc);
    }
}
