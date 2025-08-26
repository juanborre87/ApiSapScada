using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProcessOrderConfirmationMaterialMovementConfiguration : IEntityTypeConfiguration<ProcessOrderConfirmationMaterialMovement>
{
    public void Configure(EntityTypeBuilder<ProcessOrderConfirmationMaterialMovement> entity)
    {
        entity.HasKey(e => e.IdGuid).HasName("PK__ProcessO__838CF1459311A9A7");

        entity.ToTable("ProcessOrderConfirmationMaterialMovement");

        entity.HasIndex(e => e.ProcessOrderComponentIdGuid, "IX_POCMM_ProcessOrderComponentIdGuid");

        entity.HasIndex(e => e.ProcessOrderConfirmationIdGuid, "IX_POCMM_ProcessOrderConfirmationIdGuid");

        entity.Property(e => e.IdGuid).ValueGeneratedNever();
        entity.Property(e => e.EntryUnit).HasMaxLength(50);
        entity.Property(e => e.EntryUnitIsocode)
            .HasMaxLength(50)
            .HasColumnName("EntryUnitISOCode");
        entity.Property(e => e.EntryUnitSapcode)
            .HasMaxLength(50)
            .HasColumnName("EntryUnitSAPCode");
        entity.Property(e => e.GoodsMovementDateTime).HasColumnType("datetime");
        entity.Property(e => e.InterfaceCreateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.QuantityInEntryUnit).HasColumnType("decimal(18, 3)");

        entity.HasOne(d => d.ProcessOrderComponentId).WithMany(p => p.ProcessOrderConfirmationMaterialMovements)
            .HasForeignKey(d => d.ProcessOrderComponentIdGuid)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderComponent");

        entity.HasOne(d => d.ProcessOrderConfirmationId).WithMany(p => p.ProcessOrderConfirmationMaterialMovements)
            .HasForeignKey(d => d.ProcessOrderConfirmationIdGuid)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderConfirmation");
    }
}
