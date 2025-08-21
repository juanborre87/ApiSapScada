using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;


public class ProcessOrderComponentConfiguration : IEntityTypeConfiguration<ProcessOrderComponent>
{
    public void Configure(EntityTypeBuilder<ProcessOrderComponent> entity)
    {
        entity.HasKey(e => e.IdGuid).HasName("PK__ProcessO__838CF145B0B0C94D");

        entity.ToTable("ProcessOrderComponent");

        entity.Property(e => e.IdGuid).ValueGeneratedNever();
        entity.Property(e => e.Batch).HasMaxLength(50);
        entity.Property(e => e.EntryUnit).HasMaxLength(50);
        entity.Property(e => e.EntryUnitIsocode)
            .HasMaxLength(50)
            .HasColumnName("EntryUnitISOCode");
        entity.Property(e => e.EntryUnitSapcode)
            .HasMaxLength(50)
            .HasColumnName("EntryUnitSAPCode");
        entity.Property(e => e.GoodsMovementType).HasMaxLength(50);
        entity.Property(e => e.GoodsRecipientName).HasMaxLength(50);
        entity.Property(e => e.InterfaceCreateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.LastChangeDateTime).HasColumnType("datetime");
        entity.Property(e => e.ManufacturingOrder)
            .IsRequired()
            .HasMaxLength(50);
        entity.Property(e => e.Material).HasMaxLength(50);
        entity.Property(e => e.MatlCompRequirementDateTime).HasColumnType("datetime");
        entity.Property(e => e.Reservation).HasMaxLength(50);
        entity.Property(e => e.ReservationItem).HasMaxLength(50);
        entity.Property(e => e.StorageLocation).HasMaxLength(50);
        entity.Property(e => e.UnloadingPointName).HasMaxLength(50);

        entity.HasOne(d => d.ManufacturingOrderNavigation).WithMany(p => p.ProcessOrderComponents)
            .HasForeignKey(d => d.ManufacturingOrder)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessOrderComponent_ProcessOrder");

        entity.HasOne(d => d.MaterialNavigation).WithMany(p => p.ProcessOrderComponents)
            .HasForeignKey(d => d.Material)
            .HasConstraintName("FK_ProcessOrderComponent_Product");
    }
}
