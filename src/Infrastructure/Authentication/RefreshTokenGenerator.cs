using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Authentication;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

internal sealed class RefreshTokenGenerator(
    IOptions<JwtOptions> options)
    : IRefreshTokenGenerator
{
    private const int TokenSizeInBytes = 32;

    public TimeSpan Lifetime => TimeSpan.FromDays(options.Value.RefreshTokenDays);

    public string Create()
    {
        return Base64Url.EncodeToString(
            RandomNumberGenerator.GetBytes(TokenSizeInBytes));
    }

    /// <summary>
    /// A plain SHA-256 rather than a password hash: the token is 256 bits of entropy
    /// already, so there is nothing to brute force and lookups must stay deterministic.
    /// </summary>
    public string Hash(
        string token)
    {
        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token)));
    }
}
