namespace Domain.Authorization;

/// <summary>
/// Names of the roles created by the reference-data migration. Roles are only a
/// convenient bundle of permissions; authorization always checks permissions.
/// </summary>
public static class Roles
{
    public const string Admin = "admin";
    public const string User = "user";
}
