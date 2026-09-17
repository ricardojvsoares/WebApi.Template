using Domain.Common;

namespace Domain.Users.Entities;

public sealed class User : Entity
{
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Upper-invariant form of <see cref="Email" />, carrying the uniqueness constraint so
    /// that lookups never depend on the casing the caller happened to send.
    /// </summary>
    public string EmailNormalized { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public ICollection<Role> Roles { get; } = [];

    public static string NormalizeEmail(
        string email)
    {
        return email.Trim().ToUpperInvariant();
    }
}
