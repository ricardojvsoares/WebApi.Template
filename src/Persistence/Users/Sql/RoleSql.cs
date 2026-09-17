namespace Persistence.Users.Sql;

internal static class RoleSql
{
    private const string Columns = "id, name, description";

    public const string GetByName = $"""
        SELECT {Columns}
        FROM roles
        WHERE lower(name) = lower(@Name);
        """;

    public const string List = $"""
        SELECT {Columns}
        FROM roles
        ORDER BY name;
        """;

    public const string AssignToUser = """
        INSERT INTO user_roles (user_id, role_id)
        VALUES (@UserId, @RoleId)
        ON CONFLICT (user_id, role_id) DO NOTHING;
        """;

    public const string RemoveFromUser = """
        DELETE FROM user_roles
        WHERE user_id = @UserId
          AND role_id = @RoleId;
        """;

    public const string GetKnownPermissionNames = """
        SELECT name
        FROM permissions
        ORDER BY name;
        """;
}
