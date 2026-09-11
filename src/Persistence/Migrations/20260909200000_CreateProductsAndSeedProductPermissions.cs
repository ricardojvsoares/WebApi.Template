using System.Data;
using System.Globalization;
using Domain.Authorization;
using FluentMigrator;

namespace Persistence.Migrations;

[Migration(20260909200000)]
public sealed class CreateProductsAndSeedProductPermissions
    : Migration
{
    public override void Up()
    {
        Create.Table("products")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey("pk_products")
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(2000).Nullable()
            .WithColumn("price").AsFloat().NotNullable()
            .WithColumn("image").AsString(2048).NotNullable()
            .WithColumn("created_at_utc").AsCustom("timestamptz").NotNullable()
            .WithColumn("created_by").AsGuid().NotNullable()
            .WithColumn("updated_at_utc").AsCustom("timestamptz").NotNullable()
            .WithColumn("updated_by").AsGuid().NotNullable();

        Create.ForeignKey("fk_products_created_by")
            .FromTable("products").ForeignColumn("created_by")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(Rule.None);

        Create.ForeignKey("fk_products_updated_by")
            .FromTable("products").ForeignColumn("updated_by")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(Rule.None);

        Create.Index("ix_products_created_at_utc")
            .OnTable("products")
            .OnColumn("created_at_utc").Descending();

        foreach (var permission in ProductPermissions.All)
        {
            var permissionId = Guid.NewGuid();

            Execute.Sql(
                $"""
                INSERT INTO permissions (id, name, description)
                VALUES ('{permissionId}', '{permission}', '{Describe(permission)}')
                ON CONFLICT (name) DO NOTHING;
                """);

            Execute.Sql(
                $"""
                INSERT INTO role_permissions (role_id, permission_id)
                SELECT r.id, p.id
                FROM roles r
                CROSS JOIN permissions p
                WHERE r.name IN ('{Roles.Admin}', '{Roles.User}')
                  AND p.name = '{permission}'
                ON CONFLICT DO NOTHING;
                """);
        }
    }

    public override void Down()
    {
        foreach (var permission in ProductPermissions.All)
        {
            Execute.Sql(
                $"""
                DELETE FROM role_permissions
                WHERE permission_id IN (
                    SELECT id FROM permissions WHERE name = '{permission}');
                """);

            Delete.FromTable("permissions").Row(new { name = permission });
        }

        Delete.Table("products");
    }

    private static string Describe(
        string permission)
    {
        var separator = permission.IndexOf(':', StringComparison.Ordinal);

        if (separator <= 0)
        {
            return permission;
        }

        var action = permission[..separator];
        var resource = permission[(separator + 1)..];

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{char.ToUpper(action[0], CultureInfo.InvariantCulture)}{action[1..]} {resource}");
    }
}
