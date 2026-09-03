namespace Domain.Users.Entities;

public sealed class User
{
    private User() { }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Upper-invariant form of <see cref="Email" />, carrying the uniqueness constraint so
    /// that lookups never depend on the casing the caller happened to send.
    /// </summary>
    public string EmailNormalized { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public static User Create(
        string email,
        string passwordHash,
        string displayName,
        DateTime nowUtc)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            EmailNormalized = NormalizeEmail(email),
            PasswordHash = passwordHash,
            DisplayName = displayName,
            IsActive = true,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc
        };
    }

    public static string NormalizeEmail(
        string email)
    {
        return email.Trim().ToUpperInvariant();
    }

    public void UpdateProfile(
        string displayName,
        bool isActive,
        DateTime nowUtc)
    {
        DisplayName = displayName;
        IsActive = isActive;
        UpdatedAtUtc = nowUtc;
    }

    public void ChangePassword(
        string passwordHash,
        DateTime nowUtc)
    {
        PasswordHash = passwordHash;
        UpdatedAtUtc = nowUtc;
    }
}
