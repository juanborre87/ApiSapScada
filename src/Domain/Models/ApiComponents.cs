namespace Domain.Models
{
    public class ApiComponents
    {
        public Components d { get; set; }
    }

    public class Components
    {
        public List<ProcessOrderComponent> results { get; set; }
    }

    public class ProcessOrderComponent
    {
        public ComponentsMetadata __metadata { get; set; }
        public string Reservation { get; set; }
        public string ReservationItem { get; set; }
        public string ReservationRecordType { get; set; }
        public string MaterialGroup { get; set; }
        public string Material { get; set; }
        public string Plant { get; set; }
        public string ManufacturingOrderCategory { get; set; }
        public string ManufacturingOrderType { get; set; }
        public string ManufacturingOrder { get; set; }
        public string ManufacturingOrderSequence { get; set; }
        public string ManufacturingOrderOperation { get; set; }
        public string ProductionPlant { get; set; }
        public string OrderInternalBillOfOperations { get; set; }
        public string MatlCompRequirementDate { get; set; }
        public string MatlCompRequirementTime { get; set; }
        public bool ReservationIsFinallyIssued { get; set; }
        public bool MatlCompIsMarkedForDeletion { get; set; }
        public bool IsBulkMaterialComponent { get; set; }
        public bool MatlCompIsMarkedForBackflush { get; set; }
        public string MaterialCompIsCostRelevant { get; set; }
        public string OrderComponentLongText { get; set; }
        public string SalesOrder { get; set; }
        public string SalesOrderItem { get; set; }
        public string SortField { get; set; }
        public string BillOfMaterialCategory { get; set; }
        public string BOMItem { get; set; }
        public string BOMItemCategory { get; set; }
        public string BillOfMaterialItemNumber { get; set; }
        public string BOMItemDescription { get; set; }
        public string StorageLocation { get; set; }
        public string Batch { get; set; }
        public string BatchSplitType { get; set; }
        public string GoodsMovementType { get; set; }
        public string SupplyArea { get; set; }
        public string GoodsRecipientName { get; set; }
        public string UnloadingPointName { get; set; }
        public bool MaterialCompIsAlternativeItem { get; set; }
        public string AlternativeItemGroup { get; set; }
        public string AlternativeItemStrategy { get; set; }
        public string AlternativeItemPriority { get; set; }
        public string UsageProbabilityPercent { get; set; }
        public bool MaterialComponentIsPhantomItem { get; set; }
        public string LeadTimeOffset { get; set; }
        public bool QuantityIsFixed { get; set; }
        public bool IsNetScrap { get; set; }
        public string ComponentScrapInPercent { get; set; }
        public string OperationScrapInPercent { get; set; }
        public string BaseUnit { get; set; }
        public string BaseUnitISOCode { get; set; }
        public string BaseUnitSAPCode { get; set; }
        public string RequiredQuantity { get; set; }
        public string WithdrawnQuantity { get; set; }
        public string ConfirmedAvailableQuantity { get; set; }
        public string MaterialCompOriginalQuantity { get; set; }
        public string EntryUnit { get; set; }
        public string EntryUnitISOCode { get; set; }
        public string EntryUnitSAPCode { get; set; }
        public string GoodsMovementEntryQty { get; set; }
        public string Currency { get; set; }
        public string WithdrawnQuantityAmount { get; set; }
        public string LastChangeDateTime { get; set; }
    }

    public class ComponentsMetadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
        public string etag { get; set; }
    }
}
