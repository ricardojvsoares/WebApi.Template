using Domain.Todos.Entities;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class TodoConfiguration
    : IEntityTypeConfiguration<Todo>
{
    public void Configure(
        EntityTypeBuilder<Todo> builder)
    {
        builder.ToTable("todos");

        builder.HasKey(todo => todo.Id);

        builder.Property(todo => todo.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(todo => todo.Description)
            .HasMaxLength(2000);

        builder.Property(todo => todo.IsCompleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(todo => todo.CreatedAtUtc)
            .IsRequired();

        builder.Property(todo => todo.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(todo => new { todo.OwnerUserId, todo.IsCompleted })
            .HasDatabaseName("ix_todos_owner_user_id_is_completed");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(todo => todo.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_todos_owner_user");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(todo => todo.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_todos_created_by");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(todo => todo.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_todos_updated_by");
    }
}
