namespace Application.Abstractions.Security;

public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plaintext password into a self-describing string that carries the
    /// algorithm, iteration count and salt alongside the derived key.
    /// </summary>
    string Hash(
        string password);

    /// <summary>
    /// Verifies a plaintext password against a stored hash in constant time.
    /// Returns false rather than throwing when the stored hash is malformed.
    /// </summary>
    bool Verify(
        string password,
        string passwordHash);
}
