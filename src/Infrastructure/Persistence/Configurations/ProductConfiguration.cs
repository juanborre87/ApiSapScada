using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {
        entity.HasKey(e => e.ProductCode).HasName("PK__Product__2F4E024E39B27D83");

        entity.ToTable("Product");

        entity.Property(e => e.ProductCode).HasMaxLength(50);
        entity.Property(e => e.InterfaceCreateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.InterfaceUpdateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.ProductDescription).HasMaxLength(255);
        entity.Property(e => e.ProductType).HasMaxLength(100);

        entity.HasOne(d => d.CommStatusNavigation).WithMany(p => p.Products)
            .HasForeignKey(d => d.CommStatus)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Product_CommStatus");
    }
}
