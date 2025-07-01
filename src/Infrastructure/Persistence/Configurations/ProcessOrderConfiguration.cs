using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProcessOrderConfiguration : IEntityTypeConfiguration<ProcessOrder>
{
    public void Configure(EntityTypeBuilder<ProcessOrder> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__ProcessO__3214EC079F66E3DF");

        entity.ToTable("ProcessOrder");

        entity.HasIndex(e => e.ManufacturingOrder, "UQ__ProcessO__F6CEB4CF2652C798").IsUnique();

        entity.Property(e => e.GoodsRecipientName).HasMaxLength(50);
        entity.Property(e => e.InterfaceTimestamp).HasColumnType("datetime");
        entity.Property(e => e.LastChangeDateTime).HasColumnType("datetime");
        entity.Property(e => e.ManufacturingOrder)
            .IsRequired()
            .HasMaxLength(50);
        entity.Property(e => e.ManufacturingOrderCategory).HasMaxLength(50);
        entity.Property(e => e.ManufacturingOrderType).HasMaxLength(50);
        entity.Property(e => e.Material).HasMaxLength(50);
        entity.Property(e => e.MfgOrderActualReleaseDateTime).HasColumnType("datetime");
        entity.Property(e => e.MfgOrderCreationDateTime).HasColumnType("datetime");
        entity.Property(e => e.MfgOrderPlannedEndDateTime).HasColumnType("datetime");
        entity.Property(e => e.MfgOrderPlannedStartDateTime).HasColumnType("datetime");
        entity.Property(e => e.MfgOrderScheduledEndDateTime).HasColumnType("datetime");
        entity.Property(e => e.MfgOrderScheduledStartDateTime).HasColumnType("datetime");
        entity.Property(e => e.Plant).HasMaxLength(50);
        entity.Property(e => e.ProductionPlant).HasMaxLength(50);
        entity.Property(e => e.ProductionSupervisor).HasMaxLength(50);
        entity.Property(e => e.ProductionUnit).HasMaxLength(50);
        entity.Property(e => e.ProductionUnitIsocode)
            .HasMaxLength(50)
            .HasColumnName("ProductionUnitISOCode");
        entity.Property(e => e.ProductionUnitSapcode)
            .HasMaxLength(50)
            .HasColumnName("ProductionUnitSAPCode");
        entity.Property(e => e.ProductionVersion).HasMaxLength(50);
        entity.Property(e => e.StorageLocation).HasMaxLength(50);
        entity.Property(e => e.UnloadingPointName).HasMaxLength(50);

        entity.HasOne(d => d.CommStatusNavigation).WithMany(p => p.ProcessOrders)
            .HasForeignKey(d => d.CommStatus)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessOrder_CommStatus");

        entity.HasOne(d => d.MaterialNavigation).WithMany(p => p.ProcessOrders)
            .HasForeignKey(d => d.Material)
            .HasConstraintName("FK_ProcessOrder_Product");

        entity.HasOne(d => d.StatusNavigation).WithMany(p => p.ProcessOrders)
            .HasForeignKey(d => d.Status)
            .HasConstraintName("FK_ProcessOrder_Status");
    }
}
