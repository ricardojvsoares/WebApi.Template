using Application.Abstractions.Data;
using Dapper;
using Domain.Products.Entities;
using Domain.Products.Repositories;
using Persistence.Products.Sql;

namespace Persistence.Products.Repositories;

internal sealed class ProductRepository(
    INpgsqlConnectionFactory connectionFactory)
    : IProductRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory = connectionFactory;

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Product>(
            new CommandDefinition(
                ProductSql.GetById,
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Product>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var products = await connection.QueryAsync<Product>(
            new CommandDefinition(
                ProductSql.List,
                new { Skip = skip, Take = take },
                cancellationToken: cancellationToken));

        return [.. products];
    }

    public async Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ProductSql.Count,
                cancellationToken: cancellationToken));
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                ProductSql.Insert,
                product,
                cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                ProductSql.Update,
                product,
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
                ProductSql.Delete,
                new { Id = id },
                cancellationToken: cancellationToken));

        return affected > 0;
    }
}
