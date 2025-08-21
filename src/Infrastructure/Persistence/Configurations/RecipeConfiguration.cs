using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> entity)
    {
        entity.HasKey(e => e.BillOfMaterialHeaderUuid).HasName("PK__Recipe__381E66B71497D032");

        entity.ToTable("Recipe");

        entity.Property(e => e.BillOfMaterialHeaderUuid)
            .ValueGeneratedNever()
            .HasColumnName("BillOfMaterialHeaderUUID");
        entity.Property(e => e.BillOfMaterial)
            .IsRequired()
            .HasMaxLength(50);
        entity.Property(e => e.InterfaceCreateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.InterfaceUpdateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.Material)
            .IsRequired()
            .HasMaxLength(50);

        entity.HasOne(d => d.CommStatusNavigation).WithMany(p => p.Recipes)
            .HasForeignKey(d => d.CommStatus)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Recipe_CommStatus");
    }
}
