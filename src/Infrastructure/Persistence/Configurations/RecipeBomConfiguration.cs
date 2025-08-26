using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RecipeBomConfiguration : IEntityTypeConfiguration<RecipeBom>
{
    public void Configure(EntityTypeBuilder<RecipeBom> entity)
    {
        entity.HasKey(e => e.BillOfMaterialItemUuid).HasName("PK__RecipeBO__DBF6B46E4EF4E1E1");

        entity.ToTable("RecipeBOM");

        entity.HasIndex(e => e.BillOfMaterialHeaderUuid, "IX_RecipeBOM_BillOfMaterialHeaderUUID");

        entity.Property(e => e.BillOfMaterialItemUuid)
            .ValueGeneratedNever()
            .HasColumnName("BillOfMaterialItemUUID");
        entity.Property(e => e.BillOfMaterialComponent)
            .IsRequired()
            .HasMaxLength(50);
        entity.Property(e => e.BillOfMaterialHeaderUuid).HasColumnName("BillOfMaterialHeaderUUID");
        entity.Property(e => e.BillOfMaterialItemQuantity).HasColumnType("decimal(18, 3)");

        entity.HasOne(d => d.BillOfMaterialHeaderUu).WithMany(p => p.RecipeBoms)
            .HasForeignKey(d => d.BillOfMaterialHeaderUuid)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_RecipeBOM_Recipe");
    }
}
