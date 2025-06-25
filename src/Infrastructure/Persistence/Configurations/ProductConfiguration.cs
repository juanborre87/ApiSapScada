using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {
        entity.HasKey(e => e.ProductName);

        entity.ToTable("Product");

        entity.Property(e => e.ProductName)
            .HasMaxLength(50)
            .HasColumnName("Product");
        entity.Property(e => e.ProductDescription).HasMaxLength(255);
    }
}
