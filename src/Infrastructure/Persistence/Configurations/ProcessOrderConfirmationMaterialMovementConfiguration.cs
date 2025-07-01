using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProcessOrderConfirmationMaterialMovementConfiguration : IEntityTypeConfiguration<ProcessOrderConfirmationMaterialMovement>
{
    public void Configure(EntityTypeBuilder<ProcessOrderConfirmationMaterialMovement> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__ProcessO__3214EC0780D7C9BB");

        entity.ToTable("ProcessOrderConfirmationMaterialMovement");

        entity.Property(e => e.EntryUnit).HasMaxLength(50);
        entity.Property(e => e.EntryUnitIsocode)
            .HasMaxLength(50)
            .HasColumnName("EntryUnitISOCode");
        entity.Property(e => e.EntryUnitSapcode)
            .HasMaxLength(50)
            .HasColumnName("EntryUnitSAPCode");
        entity.Property(e => e.GoodsMovementDateTime).HasColumnType("datetime");
        entity.Property(e => e.InterfaceTimestamp).HasColumnType("datetime");

        entity.HasOne(d => d.ProcessOrderComponent).WithMany(p => p.ProcessOrderConfirmationMaterialMovements)
            .HasForeignKey(d => d.ProcessOrderComponentId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderComponent");

        entity.HasOne(d => d.ProcessOrderConfirmation).WithMany(p => p.ProcessOrderConfirmationMaterialMovements)
            .HasForeignKey(d => d.ProcessOrderConfirmationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderConfirmation");
    }
}
