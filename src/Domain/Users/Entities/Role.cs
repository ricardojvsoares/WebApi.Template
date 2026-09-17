namespace Domain.Users.Entities;

/// <summary>
/// A named bundle of permissions. Roles carry no authority of their own; authorization
/// always checks the permissions a role grants.
/// </summary>
public sealed class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Permission> Permissions { get; } = [];
    public ICollection<User> Users { get; } = [];
}
