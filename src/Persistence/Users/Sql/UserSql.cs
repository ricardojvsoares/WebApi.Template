namespace Persistence.Users.Sql;

internal static class UserSql
{
    private const string Columns = """
        id, email, email_normalized, password_hash, display_name, is_active,
        created_at_utc, created_by, updated_at_utc, updated_by
        """;

    public const string GetById = $"""
        SELECT {Columns}
        FROM users
        WHERE id = @Id;
        """;

    public const string GetByEmail = $"""
        SELECT {Columns}
        FROM users
        WHERE email_normalized = @EmailNormalized;
        """;

    public const string EmailExists = """
        SELECT EXISTS (
            SELECT 1
            FROM users
            WHERE email_normalized = @EmailNormalized);
        """;

    public const string List = $"""
        SELECT {Columns}
        FROM users
        ORDER BY created_at_utc DESC, id
        LIMIT @Take OFFSET @Skip;
        """;

    public const string Count = """
        SELECT COUNT(*)
        FROM users;
        """;

    public const string Insert = """
        INSERT INTO users (
            id, email, email_normalized, password_hash, display_name, is_active,
            created_at_utc, created_by, updated_at_utc, updated_by)
        VALUES (
            @Id, @Email, @EmailNormalized, @PasswordHash, @DisplayName, @IsActive,
            @CreatedAtUtc, @CreatedBy, @UpdatedAtUtc, @UpdatedBy);
        """;

    public const string Update = """
        UPDATE users
        SET email = @Email,
            email_normalized = @EmailNormalized,
            password_hash = @PasswordHash,
            display_name = @DisplayName,
            is_active = @IsActive,
            updated_at_utc = @UpdatedAtUtc,
            updated_by = @UpdatedBy
        WHERE id = @Id;
        """;

    public const string Delete = """
        DELETE FROM users
        WHERE id = @Id;
        """;

    public const string GetPermissions = """
        SELECT DISTINCT p.name
        FROM user_roles ur
        INNER JOIN role_permissions rp ON rp.role_id = ur.role_id
        INNER JOIN permissions p ON p.id = rp.permission_id
        WHERE ur.user_id = @UserId
        ORDER BY p.name;
        """;

    public const string GetRoleNames = """
        SELECT r.name
        FROM user_roles ur
        INNER JOIN roles r ON r.id = ur.role_id
        WHERE ur.user_id = @UserId
        ORDER BY r.name;
        """;
}
