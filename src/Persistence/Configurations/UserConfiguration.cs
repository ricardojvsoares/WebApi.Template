using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Seeding;

namespace Persistence.Configurations;

internal sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(
        EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(user => user.EmailNormalized)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(user => user.DisplayName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(user => user.CreatedAtUtc)
            .IsRequired();

        builder.Property(user => user.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(user => user.EmailNormalized)
            .IsUnique()
            .HasDatabaseName("uq_users_email_normalized");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(user => user.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_users_created_by");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(user => user.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_users_updated_by");

        builder.HasMany(user => user.Roles)
            .WithMany(role => role.Users)
            .UsingEntity<Dictionary<string, object>>(
                "user_roles",
                right => right
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("role_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_user_roles_role"),
                left => left
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("user_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_user_roles_user"),
                join =>
                {
                    join.HasKey("user_id", "role_id")
                        .HasName("pk_user_roles");
                    join.ToTable("user_roles");
                });
    }
}

internal sealed class RoleConfiguration
    : IEntityTypeConfiguration<Role>
{
    public void Configure(
        EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(role => role.Description)
            .HasMaxLength(256);

        builder.HasIndex(role => role.Name)
            .IsUnique()
            .HasDatabaseName("uq_roles_name");

        builder.HasData(
            ReferenceData.Roles.Select(role => new
            {
                role.Id,
                role.Name,
                role.Description
            }));

        builder.HasMany(role => role.Permissions)
            .WithMany(permission => permission.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "role_permissions",
                right => right
                    .HasOne<Permission>()
                    .WithMany()
                    .HasForeignKey("permission_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_role_permissions_permission"),
                left => left
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("role_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_role_permissions_role"),
                join =>
                {
                    join.HasKey("role_id", "permission_id")
                        .HasName("pk_role_permissions");
                    join.ToTable("role_permissions");
                    join.HasData(
                        ReferenceData.RolePermissions.Select(pair => new
                        {
                            role_id = pair.RoleId,
                            permission_id = pair.PermissionId
                        }));
                });
    }
}

internal sealed class PermissionConfiguration
    : IEntityTypeConfiguration<Permission>
{
    public void Configure(
        EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(permission => permission.Description)
            .HasMaxLength(256);

        builder.HasIndex(permission => permission.Name)
            .IsUnique()
            .HasDatabaseName("uq_permissions_name");

        builder.HasData(
            ReferenceData.Permissions.Select(permission => new
            {
                permission.Id,
                permission.Name,
                permission.Description
            }));
    }
}
