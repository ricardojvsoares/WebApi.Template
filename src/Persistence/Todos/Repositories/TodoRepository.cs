using Application.Abstractions.Data;
using Dapper;
using Domain.Todos.Entities;
using Domain.Todos.Repositories;
using Persistence.Todos.Sql;

namespace Persistence.Todos.Repositories;

internal sealed class TodoRepository(
    INpgsqlConnectionFactory connectionFactory)
    : ITodoRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory = connectionFactory;

    public async Task<Todo?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Todo>(
            new CommandDefinition(
                TodoSql.GetById,
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
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var todos = await connection.QueryAsync<Todo>(
            new CommandDefinition(
                TodoSql.List,
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
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                TodoSql.Count,
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
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                TodoSql.Insert,
                todo,
                cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        Todo todo,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                TodoSql.Update,
                todo,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(
                TodoSql.Delete,
                new { Id = id },
                cancellationToken: cancellationToken));

        return affected > 0;
    }
}
