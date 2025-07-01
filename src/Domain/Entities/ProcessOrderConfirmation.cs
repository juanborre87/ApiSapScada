namespace Domain.Entities;

public partial class ProcessOrderConfirmation
{
    public long Id { get; set; }

    public string OrderId { get; set; }

    public string ConfirmationText { get; set; }

    public string FinalConfirmationType { get; set; }

    public bool? IsFinalConfirmation { get; set; }

    public DateTime? ConfirmationEntryDateTime { get; set; }

    public string EnteredByUser { get; set; }

    public string Plant { get; set; }

    public string WorkCenter { get; set; }

    public string Personnel { get; set; }

    public DateTime? PostingDate { get; set; }

    public string ConfirmationUnit { get; set; }

    public string ConfirmationUnitIsocode { get; set; }

    public string ConfirmationUnitSapcode { get; set; }

    public float? ConfirmationYieldQuantity { get; set; }

    public float? ConfirmationScrapQuantity { get; set; }

    public string VarianceReasonCode { get; set; }

    public string Batch { get; set; }

    public DateTime? Expiration { get; set; }

    public byte? Sapresponse { get; set; }

    public byte CommStatus { get; set; }

    public DateTime? InterfaceTimestamp { get; set; }

    public virtual CommStatus CommStatusNavigation { get; set; }

    public virtual ProcessOrder Order { get; set; }

    public virtual ICollection<ProcessOrderConfirmationMaterialMovement> ProcessOrderConfirmationMaterialMovements { get; set; } = new List<ProcessOrderConfirmationMaterialMovement>();
}
