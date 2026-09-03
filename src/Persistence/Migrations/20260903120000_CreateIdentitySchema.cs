using FluentMigrator;

namespace Persistence.Migrations;

[Migration(20260903120000)]
public sealed class CreateIdentitySchema
    : Migration
{
    public override void Up()
    {
        Create.Table("users")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey("pk_users")
            .WithColumn("email").AsString(256).NotNullable()
            // Uniqueness lives on the normalized column so two accounts cannot differ by
            // casing alone, while the original casing is preserved for display.
            .WithColumn("email_normalized").AsString(256).NotNullable()
            .WithColumn("password_hash").AsString(512).NotNullable()
            .WithColumn("display_name").AsString(128).NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at_utc").AsCustom("timestamptz").NotNullable()
            .WithColumn("updated_at_utc").AsCustom("timestamptz").NotNullable();

        Create.UniqueConstraint("uq_users_email_normalized")
            .OnTable("users")
            .Column("email_normalized");

        Create.Table("roles")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey("pk_roles")
            .WithColumn("name").AsString(64).NotNullable()
            .WithColumn("description").AsString(256).Nullable();

        Create.UniqueConstraint("uq_roles_name")
            .OnTable("roles")
            .Column("name");

        Create.Table("permissions")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey("pk_permissions")
            .WithColumn("name").AsString(128).NotNullable()
            .WithColumn("description").AsString(256).Nullable();

        Create.UniqueConstraint("uq_permissions_name")
            .OnTable("permissions")
            .Column("name");

        Create.Table("role_permissions")
            .WithColumn("role_id").AsGuid().NotNullable()
            .WithColumn("permission_id").AsGuid().NotNullable();

        Create.PrimaryKey("pk_role_permissions")
            .OnTable("role_permissions")
            .Columns("role_id", "permission_id");

        Create.ForeignKey("fk_role_permissions_role")
            .FromTable("role_permissions").ForeignColumn("role_id")
            .ToTable("roles").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_role_permissions_permission")
            .FromTable("role_permissions").ForeignColumn("permission_id")
            .ToTable("permissions").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Table("user_roles")
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("role_id").AsGuid().NotNullable();

        Create.PrimaryKey("pk_user_roles")
            .OnTable("user_roles")
            .Columns("user_id", "role_id");

        Create.ForeignKey("fk_user_roles_user")
            .FromTable("user_roles").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_user_roles_role")
            .FromTable("user_roles").ForeignColumn("role_id")
            .ToTable("roles").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Table("refresh_tokens")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey("pk_refresh_tokens")
            .WithColumn("user_id").AsGuid().NotNullable()
            // Only the hash is stored, so a database leak does not yield usable tokens.
            .WithColumn("token_hash").AsString(128).NotNullable()
            .WithColumn("expires_at_utc").AsCustom("timestamptz").NotNullable()
            .WithColumn("revoked_at_utc").AsCustom("timestamptz").Nullable()
            .WithColumn("replaced_by_token_hash").AsString(128).Nullable()
            .WithColumn("created_at_utc").AsCustom("timestamptz").NotNullable();

        Create.UniqueConstraint("uq_refresh_tokens_token_hash")
            .OnTable("refresh_tokens")
            .Column("token_hash");

        Create.ForeignKey("fk_refresh_tokens_user")
            .FromTable("refresh_tokens").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("ix_refresh_tokens_user_id")
            .OnTable("refresh_tokens")
            .OnColumn("user_id").Ascending();
    }

    public override void Down()
    {
        Delete.Table("refresh_tokens");
        Delete.Table("user_roles");
        Delete.Table("role_permissions");
        Delete.Table("permissions");
        Delete.Table("roles");
        Delete.Table("users");
    }
}
