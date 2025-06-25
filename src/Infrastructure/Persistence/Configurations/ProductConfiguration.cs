using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {
        entity.HasKey(e => e.Product1);

        entity.ToTable("Product");

        entity.Property(e => e.Product1)
            .HasMaxLength(50)
            .HasColumnName("Product");
        entity.Property(e => e.ProductDescription).HasMaxLength(255);
    }
}
