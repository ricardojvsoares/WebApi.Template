using Application.Abstractions.Data;
using Dapper;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;

namespace Persistence.Todos.Repositories;

internal sealed class TodoRepository(
    INpgsqlConnectionFactory connectionFactory)
    : ITodoRepository
{
    private const string SelectColumns = """
        id, title, description, is_completed, due_date_utc, completed_at_utc,
        owner_user_id, created_at_utc, updated_at_utc
        """;

    private readonly INpgsqlConnectionFactory _connectionFactory = connectionFactory;

    public async Task<Todo?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM todos
            WHERE id = @Id;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Todo>(
            new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Todo>> ListAsync(
        Guid? ownerUserId,
        bool? isCompleted,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        // The ::type casts let Postgres resolve the parameter types when a filter is null,
        // which it cannot infer from an untyped NULL on its own.
        const string sql = $"""
            SELECT {SelectColumns}
            FROM todos
            WHERE (@OwnerUserId::uuid IS NULL OR owner_user_id = @OwnerUserId::uuid)
              AND (@IsCompleted::boolean IS NULL OR is_completed = @IsCompleted::boolean)
            ORDER BY created_at_utc DESC, id
            LIMIT @Take OFFSET @Skip;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var todos = await connection.QueryAsync<Todo>(
            new CommandDefinition(
                sql,
                new
                {
                    OwnerUserId = ownerUserId,
                    IsCompleted = isCompleted,
                    Skip = skip,
                    Take = take
                },
                cancellationToken: cancellationToken));

        return [.. todos];
    }

    public async Task<int> CountAsync(
        Guid? ownerUserId,
        bool? isCompleted,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM todos
            WHERE (@OwnerUserId::uuid IS NULL OR owner_user_id = @OwnerUserId::uuid)
              AND (@IsCompleted::boolean IS NULL OR is_completed = @IsCompleted::boolean);
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    OwnerUserId = ownerUserId,
                    IsCompleted = isCompleted
                },
                cancellationToken: cancellationToken));
    }

    public async Task AddAsync(
        Todo todo,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO todos (
                id, title, description, is_completed, due_date_utc, completed_at_utc,
                owner_user_id, created_at_utc, updated_at_utc)
            VALUES (
                @Id, @Title, @Description, @IsCompleted, @DueDateUtc, @CompletedAtUtc,
                @OwnerUserId, @CreatedAtUtc, @UpdatedAtUtc);
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                todo,
                cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        Todo todo,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE todos
            SET title = @Title,
                description = @Description,
                is_completed = @IsCompleted,
                due_date_utc = @DueDateUtc,
                completed_at_utc = @CompletedAtUtc,
                updated_at_utc = @UpdatedAtUtc
            WHERE id = @Id;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                todo,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM todos
            WHERE id = @Id;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken));

        return affected > 0;
    }
}
