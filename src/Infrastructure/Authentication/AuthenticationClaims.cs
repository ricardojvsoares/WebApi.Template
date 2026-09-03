namespace Infrastructure.Authentication;

/// <summary>
/// Claim types shared by the token generator and the authorization handler. Kept as short
/// unmapped names, because inbound claim mapping is turned off when validating tokens.
/// </summary>
public static class AuthenticationClaims
{
    public const string Subject = "sub";
    public const string Email = "email";
    public const string Name = "name";
    public const string Role = "role";

    /// <summary>
    /// One claim of this type is emitted per granted permission, which is what lets an
    /// authorization decision avoid a database round trip.
    /// </summary>
    public const string Permission = "permission";
}
