namespace Domain.Users.Entities;

/// <summary>
/// A named authorization permission stored in the database. Authority checks use the
/// permission name string, never the row id.
/// </summary>
#pragma warning disable CA1711 // Name ends with Permission by design for the domain concept
public sealed class Permission
#pragma warning restore CA1711
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Role> Roles { get; } = [];
}
