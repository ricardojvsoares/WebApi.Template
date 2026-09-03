namespace Domain.Users.Entities;

/// <summary>
/// A named bundle of permissions. Roles carry no authority of their own; authorization
/// always checks the permissions a role grants.
/// </summary>
public sealed class Role
{
    private Role() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public static Role Create(
        string name,
        string? description)
    {
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
    }
}
