namespace Domain.Entities;

public partial class ProcessOrderConfirmationMaterialMovement
{
    public Guid IdGuid { get; set; }

    public Guid ProcessOrderConfirmationIdGuid { get; set; }

    public Guid ProcessOrderComponentIdGuid { get; set; }

    public string EntryUnit { get; set; }

    public string EntryUnitIsocode { get; set; }

    public string EntryUnitSapcode { get; set; }

    public decimal? QuantityInEntryUnit { get; set; }

    public DateTime? GoodsMovementDateTime { get; set; }

    public DateTime? InterfaceCreateTimestamp { get; set; }

    public virtual ProcessOrderComponent ProcessOrderComponentId { get; set; }

    public virtual ProcessOrderConfirmation ProcessOrderConfirmationId { get; set; }
}
