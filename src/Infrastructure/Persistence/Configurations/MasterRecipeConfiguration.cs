using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MasterRecipeConfiguration : IEntityTypeConfiguration<MasterRecipe>
{
    public void Configure(EntityTypeBuilder<MasterRecipe> entity)
    {
        entity.HasKey(e => e.IdGuid).HasName("PK__MasterRe__838CF1458225DDA5");

        entity.ToTable("MasterRecipe");

        entity.Property(e => e.IdGuid).ValueGeneratedNever();
        entity.Property(e => e.BillOfMaterialComponent)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.InterfaceCreateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.InterfaceUpdateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.ManufacturingOrder)
            .IsRequired()
            .HasMaxLength(50);

        entity.HasOne(d => d.ManufacturingOrderNavigation).WithMany(p => p.MasterRecipes)
            .HasForeignKey(d => d.ManufacturingOrder)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MasterRecipe_ProcessOrder");
    }
}
