namespace Domain.Entities;

public partial class ProcessOrderComponent
{
    public long Id { get; set; }

    public string ManufacturingOrder { get; set; }

    public string Material { get; set; }

    public string Reservation { get; set; }

    public string ReservationItem { get; set; }

    public DateTime? MatlCompRequirementDateTime { get; set; }

    public string StorageLocation { get; set; }

    public string Batch { get; set; }

    public string GoodsMovementType { get; set; }

    public string GoodsRecipientName { get; set; }

    public string UnloadingPointName { get; set; }

    public string EntryUnit { get; set; }

    public string EntryUnitIsocode { get; set; }

    public string EntryUnitSapcode { get; set; }

    public float? GoodsMovementEntryQty { get; set; }

    public DateTime? LastChangeDateTime { get; set; }

    public virtual ProcessOrder ManufacturingOrderNavigation { get; set; }

    public virtual Product MaterialNavigation { get; set; }

    public virtual ICollection<ProcessOrderConfirmationMaterialMovement> ProcessOrderConfirmationMaterialMovements { get; set; } = new List<ProcessOrderConfirmationMaterialMovement>();
}
