namespace Application.Abstractions.Authentication;

public interface IRefreshTokenGenerator
{
    /// <summary>
    /// Creates a new opaque refresh token. The caller hands the token to the client and
    /// persists only <see cref="Hash" /> of it.
    /// </summary>
    string Create();

    /// <summary>
    /// Hashes a refresh token for storage and lookup. Deterministic, so a presented token
    /// can be found without ever storing the token itself.
    /// </summary>
    string Hash(
        string token);

    TimeSpan Lifetime { get; }
}
