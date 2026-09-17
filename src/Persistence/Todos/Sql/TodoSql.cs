namespace Persistence.Todos.Sql;

internal static class TodoSql
{
    private const string Columns = """
        id, title, description, is_completed, due_date_utc, completed_at_utc,
        owner_user_id, created_at_utc, created_by, updated_at_utc, updated_by
        """;

    public const string GetById = $"""
        SELECT {Columns}
        FROM todos
        WHERE id = @Id;
        """;

    // ::type casts let Postgres resolve parameter types when a filter is null.
    public const string List = $"""
        SELECT {Columns}
        FROM todos
        WHERE (@OwnerUserId::uuid IS NULL OR owner_user_id = @OwnerUserId::uuid)
          AND (@IsCompleted::boolean IS NULL OR is_completed = @IsCompleted::boolean)
        ORDER BY created_at_utc DESC, id
        LIMIT @Take OFFSET @Skip;
        """;

    public const string Count = """
        SELECT COUNT(*)
        FROM todos
        WHERE (@OwnerUserId::uuid IS NULL OR owner_user_id = @OwnerUserId::uuid)
          AND (@IsCompleted::boolean IS NULL OR is_completed = @IsCompleted::boolean);
        """;

    public const string Insert = """
        INSERT INTO todos (
            id, title, description, is_completed, due_date_utc, completed_at_utc,
            owner_user_id, created_at_utc, created_by, updated_at_utc, updated_by)
        VALUES (
            @Id, @Title, @Description, @IsCompleted, @DueDateUtc, @CompletedAtUtc,
            @OwnerUserId, @CreatedAtUtc, @CreatedBy, @UpdatedAtUtc, @UpdatedBy);
        """;

    public const string Update = """
        UPDATE todos
        SET title = @Title,
            description = @Description,
            is_completed = @IsCompleted,
            due_date_utc = @DueDateUtc,
            completed_at_utc = @CompletedAtUtc,
            updated_at_utc = @UpdatedAtUtc,
            updated_by = @UpdatedBy
        WHERE id = @Id;
        """;

    public const string Delete = """
        DELETE FROM todos
        WHERE id = @Id;
        """;
}
