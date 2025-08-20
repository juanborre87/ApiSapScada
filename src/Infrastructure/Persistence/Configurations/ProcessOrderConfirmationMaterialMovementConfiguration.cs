using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProcessOrderConfirmationMaterialMovementConfiguration : IEntityTypeConfiguration<ProcessOrderConfirmationMaterialMovement>
{
    public void Configure(EntityTypeBuilder<ProcessOrderConfirmationMaterialMovement> entity)
    {
        entity.HasKey(e => e.IdGuid).HasName("PK__ProcessO__838CF145212CEBFA");

        entity.ToTable("ProcessOrderConfirmationMaterialMovement");

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
        entity.Property(e => e.InterfaceUpdateTimestamp).HasColumnType("datetime");

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
