using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProcessOrderConfirmationConfiguration : IEntityTypeConfiguration<ProcessOrderConfirmation>
{
    public void Configure(EntityTypeBuilder<ProcessOrderConfirmation> entity)
    {
        entity.HasKey(e => e.IdGuid).HasName("PK__ProcessO__838CF1458460DFE8");

        entity.ToTable("ProcessOrderConfirmation");

        entity.HasIndex(e => e.CommStatus, "IX_ProcessOrderConfirmation_CommStatus");

        entity.HasIndex(e => e.OrderId, "IX_ProcessOrderConfirmation_OrderId");

        entity.Property(e => e.IdGuid).ValueGeneratedNever();
        entity.Property(e => e.Batch).HasMaxLength(50);
        entity.Property(e => e.ConfirmationEntryDateTime).HasColumnType("datetime");
        entity.Property(e => e.ConfirmationScrapQuantity).HasColumnType("decimal(18, 3)");
        entity.Property(e => e.ConfirmationUnit).HasMaxLength(50);
        entity.Property(e => e.ConfirmationUnitIsocode)
            .HasMaxLength(50)
            .HasColumnName("ConfirmationUnitISOCode");
        entity.Property(e => e.ConfirmationUnitSapcode)
            .HasMaxLength(50)
            .HasColumnName("ConfirmationUnitSAPCode");
        entity.Property(e => e.ConfirmationYieldQuantity).HasColumnType("decimal(18, 3)");
        entity.Property(e => e.EnteredByUser).HasMaxLength(255);
        entity.Property(e => e.Expiration).HasColumnType("datetime");
        entity.Property(e => e.FinalConfirmationType).HasMaxLength(50);
        entity.Property(e => e.InterfaceCreateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.InterfaceUpdateTimestamp).HasColumnType("datetime");
        entity.Property(e => e.OrderId)
            .IsRequired()
            .HasMaxLength(50);
        entity.Property(e => e.Personnel).HasMaxLength(50);
        entity.Property(e => e.Plant).HasMaxLength(50);
        entity.Property(e => e.PostingDate).HasColumnType("datetime");
        entity.Property(e => e.Sapresponse)
            .IsUnicode(false)
            .HasColumnName("SAPResponse");
        entity.Property(e => e.VarianceReasonCode).HasMaxLength(50);
        entity.Property(e => e.WorkCenter).HasMaxLength(50);

        entity.HasOne(d => d.CommStatusNavigation).WithMany(p => p.ProcessOrderConfirmations)
            .HasForeignKey(d => d.CommStatus)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessOrderConfirmation_CommStatus");

        entity.HasOne(d => d.Order).WithMany(p => p.ProcessOrderConfirmations)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessOrderConfirmation_ProcessOrder");
    }
}
