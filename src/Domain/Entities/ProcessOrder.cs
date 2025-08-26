namespace Domain.Entities;

public partial class ProcessOrder
{
    public string ManufacturingOrder { get; set; }

    public string ManufacturingOrderCategory { get; set; }

    public string ManufacturingOrderType { get; set; }

    public string OrderLongText { get; set; }

    public int? ManufacturingOrderImportance { get; set; }

    public DateTime? MfgOrderCreationDateTime { get; set; }

    public DateTime? LastChangeDateTime { get; set; }

    public string Material { get; set; }

    public string StorageLocation { get; set; }

    public string GoodsRecipientName { get; set; }

    public string UnloadingPointName { get; set; }

    public string ProductionPlant { get; set; }

    public string Plant { get; set; }

    public string ProductionSupervisor { get; set; }

    public string ProductionVersion { get; set; }

    public DateTime? MfgOrderPlannedStartDateTime { get; set; }

    public DateTime? MfgOrderPlannedEndDateTime { get; set; }

    public DateTime? MfgOrderScheduledStartDateTime { get; set; }

    public DateTime? MfgOrderScheduledEndDateTime { get; set; }

    public DateTime? MfgOrderActualReleaseDateTime { get; set; }

    public string ProductionUnit { get; set; }

    public string ProductionUnitIsocode { get; set; }

    public string ProductionUnitSapcode { get; set; }

    public decimal? TotalQuantity { get; set; }

    public decimal? MfgOrderPlannedScrapQty { get; set; }

    public decimal? MfgOrderConfirmedYieldQty { get; set; }

    public byte? Status { get; set; }

    public byte CommStatus { get; set; }

    public DateTime? InterfaceCreateTimestamp { get; set; }

    public DateTime? InterfaceUpdateTimestamp { get; set; }

    public int? DestinoRecetaDeControl { get; set; }

    public Guid BillOfMaterialHeaderUuid { get; set; }

    public virtual Recipe BillOfMaterialHeaderUu { get; set; }

    public virtual CommStatus CommStatusNavigation { get; set; }

    public virtual Product MaterialNavigation { get; set; }

    public virtual ICollection<ProcessOrderComponent> ProcessOrderComponents { get; set; } = new List<ProcessOrderComponent>();

    public virtual ICollection<ProcessOrderConfirmation> ProcessOrderConfirmations { get; set; } = new List<ProcessOrderConfirmation>();

    public virtual ProcessOrderStatus StatusNavigation { get; set; }
}
