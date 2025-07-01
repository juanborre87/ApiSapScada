namespace Domain.Entities;

public partial class ProcessOrderConfirmationMaterialMovement
{
    public long Id { get; set; }

    public long ProcessOrderConfirmationId { get; set; }

    public long ProcessOrderComponentId { get; set; }

    public string EntryUnit { get; set; }

    public string EntryUnitIsocode { get; set; }

    public string EntryUnitSapcode { get; set; }

    public float? QuantityInEntryUnit { get; set; }

    public DateTime? GoodsMovementDateTime { get; set; }

    public DateTime? InterfaceTimestamp { get; set; }

    public virtual ProcessOrderComponent ProcessOrderComponent { get; set; }

    public virtual ProcessOrderConfirmation ProcessOrderConfirmation { get; set; }
}
