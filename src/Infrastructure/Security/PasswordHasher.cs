using System.Globalization;
using System.Security.Cryptography;
using Application.Abstractions.Security;

namespace Infrastructure.Security;

/// <summary>
/// PBKDF2-HMAC-SHA256 password hashing using only the BCL. The stored value carries the
/// algorithm, iteration count and salt, so the work factor can be raised later without
/// invalidating existing hashes.
/// </summary>
internal sealed class PasswordHasher
    : IPasswordHasher
{
    private const string Prefix = "pbkdf2-sha256";
    private const char Delimiter = '$';
    private const int SaltSizeInBytes = 16;
    private const int KeySizeInBytes = 32;

    /// <summary>OWASP's current floor for PBKDF2-HMAC-SHA256.</summary>
    private const int Iterations = 210_000;

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Hash(
        string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSizeInBytes);

        var key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            Algorithm,
            KeySizeInBytes);

        return string.Join(
            Delimiter,
            Prefix,
            Iterations.ToString(CultureInfo.InvariantCulture),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(key));
    }

    public bool Verify(
        string password,
        string passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash))
        {
            return false;
        }

        var parts = passwordHash.Split(Delimiter);

        // A malformed stored hash is treated as a failed verification rather than an
        // exception, so one corrupt row cannot turn a login into a 500.
        if (parts.Length != 4 || !string.Equals(parts[0], Prefix, StringComparison.Ordinal))
        {
            return false;
        }

        if (!int.TryParse(parts[1], CultureInfo.InvariantCulture, out int iterations) || iterations < 1)
        {
            return false;
        }

        byte[] salt;
        byte[] expectedKey;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedKey = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (salt.Length == 0 || expectedKey.Length == 0)
        {
            return false;
        }

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            Algorithm,
            expectedKey.Length);

        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }
}
