using Application.Abstractions.Data;
using Dapper;
using Domain.Products.Entities;
using Domain.Products.Repositories;

namespace Persistence.Products.Repositories;

internal sealed class ProductRepository(
    INpgsqlConnectionFactory connectionFactory)
    : IProductRepository
{
    private const string SelectColumns = """
        id, name, description, price, image,
        created_at_utc, created_by, updated_at_utc, updated_by
        """;

    private readonly INpgsqlConnectionFactory _connectionFactory = connectionFactory;

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM products
            WHERE id = @Id;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Product>(
            new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Product>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM products
            ORDER BY created_at_utc DESC, id
            LIMIT @Take OFFSET @Skip;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        var products = await connection.QueryAsync<Product>(
            new CommandDefinition(
                sql,
                new
                {
                    Skip = skip,
                    Take = take
                },
                cancellationToken: cancellationToken));

        return [.. products];
    }

    public async Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM products;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                cancellationToken: cancellationToken));
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO products (
                id, name, description, price, image,
                created_at_utc, created_by, updated_at_utc, updated_by)
            VALUES (
                @Id, @Name, @Description, @Price, @Image,
                @CreatedAtUtc, @CreatedBy, @UpdatedAtUtc, @UpdatedBy);
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                product,
                cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE products
            SET name = @Name,
                description = @Description,
                price = @Price,
                image = @Image,
                updated_at_utc = @UpdatedAtUtc,
                updated_by = @UpdatedBy
            WHERE id = @Id;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(
            cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                product,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM products
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
