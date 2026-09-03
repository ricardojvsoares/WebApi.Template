namespace Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>Minimum key length for HMAC-SHA256, in bytes.</summary>
    public const int MinimumSigningKeyBytes = 32;

    public string Issuer { get; set; } = "webapi";

    public string Audience { get; set; } = "webapi";

    public string SigningKey { get; set; } = string.Empty;

    public int AccessTokenMinutes { get; set; } = 15;

    public int RefreshTokenDays { get; set; } = 14;
}
