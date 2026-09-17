using Domain.Products.Entities;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class ProductConfiguration
    : IEntityTypeConfiguration<Product>
{
    public void Configure(
        EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasMaxLength(2000);

        builder.Property(product => product.Price)
            .IsRequired();

        builder.Property(product => product.Image)
            .HasConversion(
                uri => uri.AbsoluteUri,
                value => new Uri(value, UriKind.Absolute))
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(product => product.CreatedAtUtc)
            .IsRequired();

        builder.Property(product => product.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(product => product.CreatedAtUtc)
            .IsDescending()
            .HasDatabaseName("ix_products_created_at_utc");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(product => product.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_products_created_by");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(product => product.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_products_updated_by");
    }
}
