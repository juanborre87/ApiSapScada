using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProcessOrderConfirmationConfiguration : IEntityTypeConfiguration<ProcessOrderConfirmation>
{
    public void Configure(EntityTypeBuilder<ProcessOrderConfirmation> entity)
    {
        entity.ToTable("ProcessOrderConfirmation");

        entity.Property(e => e.ProcessOrderConfirmationId).ValueGeneratedNever();
        entity.Property(e => e.Batch).HasMaxLength(50);
        entity.Property(e => e.ConfirmationEntryDateTime).HasColumnType("datetime");
        entity.Property(e => e.ConfirmationUnit).HasMaxLength(50);
        entity.Property(e => e.ConfirmationUnitIsocode)
            .HasMaxLength(50)
            .HasColumnName("ConfirmationUnitISOCode");
        entity.Property(e => e.ConfirmationUnitSapcode)
            .HasMaxLength(50)
            .HasColumnName("ConfirmationUnitSAPCode");
        entity.Property(e => e.EnteredByUser).HasMaxLength(255);
        entity.Property(e => e.Expiration).HasColumnType("datetime");
        entity.Property(e => e.FinalConfirmationType).HasMaxLength(50);
        entity.Property(e => e.OrderId).HasColumnName("OrderID");
        entity.Property(e => e.Personnel).HasMaxLength(50);
        entity.Property(e => e.Plant).HasMaxLength(50);
        entity.Property(e => e.PostingDate).HasColumnType("datetime");
        entity.Property(e => e.VarianceReasonCode).HasMaxLength(50);
        entity.Property(e => e.WorkCenter).HasMaxLength(50);

        entity.HasOne(d => d.Order).WithMany(p => p.ProcessOrderConfirmations)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessOrderConfirmation_ProcessOrder");

        entity.HasOne(d => d.StatusNavigation).WithMany(p => p.ProcessOrderConfirmations)
            .HasForeignKey(d => d.Status)
            .HasConstraintName("FK_ProcessOrderConfirmation_OrderConfirmationStatus");
    }
}
