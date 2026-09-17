namespace Persistence.Products.Sql;

internal static class ProductSql
{
    private const string Columns = """
        id, name, description, price, image,
        created_at_utc, created_by, updated_at_utc, updated_by
        """;

    public const string GetById = $"""
        SELECT {Columns}
        FROM products
        WHERE id = @Id;
        """;

    public const string List = $"""
        SELECT {Columns}
        FROM products
        ORDER BY created_at_utc DESC, id
        LIMIT @Take OFFSET @Skip;
        """;

    public const string Count = """
        SELECT COUNT(*)
        FROM products;
        """;

    public const string Insert = """
        INSERT INTO products (
            id, name, description, price, image,
            created_at_utc, created_by, updated_at_utc, updated_by)
        VALUES (
            @Id, @Name, @Description, @Price, @Image,
            @CreatedAtUtc, @CreatedBy, @UpdatedAtUtc, @UpdatedBy);
        """;

    public const string Update = """
        UPDATE products
        SET name = @Name,
            description = @Description,
            price = @Price,
            image = @Image,
            updated_at_utc = @UpdatedAtUtc,
            updated_by = @UpdatedBy
        WHERE id = @Id;
        """;

    public const string Delete = """
        DELETE FROM products
        WHERE id = @Id;
        """;
}
