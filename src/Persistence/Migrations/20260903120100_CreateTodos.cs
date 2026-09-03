using FluentMigrator;

namespace Persistence.Migrations;

[Migration(20260903120100)]
public sealed class CreateTodos
    : Migration
{
    public override void Up()
    {
        Create.Table("todos")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey("pk_todos")
            .WithColumn("title").AsString(200).NotNullable()
            .WithColumn("description").AsString(2000).Nullable()
            .WithColumn("is_completed").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("due_date_utc").AsCustom("timestamptz").Nullable()
            .WithColumn("completed_at_utc").AsCustom("timestamptz").Nullable()
            .WithColumn("owner_user_id").AsGuid().NotNullable()
            .WithColumn("created_at_utc").AsCustom("timestamptz").NotNullable()
            .WithColumn("updated_at_utc").AsCustom("timestamptz").NotNullable();

        Create.ForeignKey("fk_todos_owner_user")
            .FromTable("todos").ForeignColumn("owner_user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        // Every todo query filters by owner, most of them also by completion state.
        Create.Index("ix_todos_owner_user_id_is_completed")
            .OnTable("todos")
            .OnColumn("owner_user_id").Ascending()
            .OnColumn("is_completed").Ascending();
    }

    public override void Down()
    {
        Delete.Table("todos");
    }
}
