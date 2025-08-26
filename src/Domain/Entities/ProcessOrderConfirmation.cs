namespace Domain.Entities;

public partial class ProcessOrderConfirmation
{
    public Guid IdGuid { get; set; }

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

    public decimal? ConfirmationYieldQuantity { get; set; }

    public decimal? ConfirmationScrapQuantity { get; set; }

    public string VarianceReasonCode { get; set; }

    public string Batch { get; set; }

    public DateTime? Expiration { get; set; }

    public string Sapresponse { get; set; }

    public byte CommStatus { get; set; }

    public DateTime? InterfaceCreateTimestamp { get; set; }

    public DateTime? InterfaceUpdateTimestamp { get; set; }

    public virtual CommStatus CommStatusNavigation { get; set; }

    public virtual ProcessOrder Order { get; set; }

    public virtual ICollection<ProcessOrderConfirmationMaterialMovement> ProcessOrderConfirmationMaterialMovements { get; set; } = new List<ProcessOrderConfirmationMaterialMovement>();
}
