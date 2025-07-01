using Newtonsoft.Json;

namespace Domain.Dtos
{

    public class OrderComponentDto
    {
        [JsonProperty("results")]
        public List<OrderComponentResultDto> Results { get; set; }
    }

    public class OrderComponentResultDto
    {
        [JsonProperty("Reservation")]
        public string Reservation { get; set; }

        [JsonProperty("ReservationItem")]
        public string ReservationItem { get; set; }

        [JsonProperty("ReservationRecordType")]
        public string ReservationRecordType { get; set; }

        [JsonProperty("MaterialGroup")]
        public string MaterialGroup { get; set; }

        [JsonProperty("Material")]
        public string Material { get; set; }

        [JsonProperty("Plant")]
        public string Plant { get; set; }

        [JsonProperty("ManufacturingOrderCategory")]
        public string ManufacturingOrderCategory { get; set; }

        [JsonProperty("ManufacturingOrderType")]
        public string ManufacturingOrderType { get; set; }

        [JsonProperty("ManufacturingOrder")]
        public string ManufacturingOrder { get; set; }

        [JsonProperty("ManufacturingOrderSequence")]
        public string ManufacturingOrderSequence { get; set; }

        [JsonProperty("ManufacturingOrderOperation")]
        public string ManufacturingOrderOperation { get; set; }

        [JsonProperty("ProductionPlant")]
        public string ProductionPlant { get; set; }

        [JsonProperty("OrderInternalBillOfOperations")]
        public string OrderInternalBillOfOperations { get; set; }

        [JsonProperty("MatlCompRequirementDate")]
        public string MatlCompRequirementDate { get; set; }

        [JsonProperty("MatlCompRequirementTime")]
        public string MatlCompRequirementTime { get; set; }

        [JsonProperty("ReservationIsFinallyIssued")]
        public bool ReservationIsFinallyIssued { get; set; }

        [JsonProperty("MatlCompIsMarkedForDeletion")]
        public bool MatlCompIsMarkedForDeletion { get; set; }

        [JsonProperty("IsBulkMaterialComponent")]
        public bool IsBulkMaterialComponent { get; set; }

        [JsonProperty("MatlCompIsMarkedForBackflush")]
        public bool MatlCompIsMarkedForBackflush { get; set; }

        [JsonProperty("MaterialCompIsCostRelevant")]
        public string MaterialCompIsCostRelevant { get; set; }

        [JsonProperty("OrderComponentLongText")]
        public string OrderComponentLongText { get; set; }

        [JsonProperty("SalesOrder")]
        public string SalesOrder { get; set; }

        [JsonProperty("SalesOrderItem")]
        public string SalesOrderItem { get; set; }

        [JsonProperty("BillOfMaterialCategory")]
        public string BillOfMaterialCategory { get; set; }

        [JsonProperty("BOMItem")]
        public string BOMItem { get; set; }

        [JsonProperty("BOMItemCategory")]
        public string BOMItemCategory { get; set; }

        [JsonProperty("BillOfMaterialItemNumber")]
        public string BillOfMaterialItemNumber { get; set; }

        [JsonProperty("BOMItemDescription")]
        public string BOMItemDescription { get; set; }

        [JsonProperty("StorageLocation")]
        public string StorageLocation { get; set; }

        [JsonProperty("Batch")]
        public string Batch { get; set; }

        [JsonProperty("BatchSplitType")]
        public string BatchSplitType { get; set; }

        [JsonProperty("GoodsMovementType")]
        public string GoodsMovementType { get; set; }

        [JsonProperty("SupplyArea")]
        public string SupplyArea { get; set; }

        [JsonProperty("GoodsRecipientName")]
        public string GoodsRecipientName { get; set; }

        [JsonProperty("UnloadingPointName")]
        public string UnloadingPointName { get; set; }

        [JsonProperty("MaterialCompIsAlternativeItem")]
        public bool MaterialCompIsAlternativeItem { get; set; }

        [JsonProperty("AlternativeItemGroup")]
        public string AlternativeItemGroup { get; set; }

        [JsonProperty("AlternativeItemStrategy")]
        public string AlternativeItemStrategy { get; set; }

        [JsonProperty("AlternativeItemPriority")]
        public string AlternativeItemPriority { get; set; }

        [JsonProperty("UsageProbabilityPercent")]
        public string UsageProbabilityPercent { get; set; }

        [JsonProperty("MaterialComponentIsPhantomItem")]
        public bool MaterialComponentIsPhantomItem { get; set; }

        [JsonProperty("LeadTimeOffset")]
        public string LeadTimeOffset { get; set; }

        [JsonProperty("QuantityIsFixed")]
        public bool QuantityIsFixed { get; set; }

        [JsonProperty("IsNetScrap")]
        public bool IsNetScrap { get; set; }

        [JsonProperty("ComponentScrapInPercent")]
        public string ComponentScrapInPercent { get; set; }

        [JsonProperty("OperationScrapInPercent")]
        public string OperationScrapInPercent { get; set; }

        [JsonProperty("BaseUnit")]
        public string BaseUnit { get; set; }

        [JsonProperty("BaseUnitISOCode")]
        public string BaseUnitISOCode { get; set; }

        [JsonProperty("BaseUnitSAPCode")]
        public string BaseUnitSAPCode { get; set; }

        [JsonProperty("RequiredQuantity")]
        public string RequiredQuantity { get; set; }

        [JsonProperty("WithdrawnQuantity")]
        public string WithdrawnQuantity { get; set; }

        [JsonProperty("ConfirmedAvailableQuantity")]
        public string ConfirmedAvailableQuantity { get; set; }

        [JsonProperty("MaterialCompOriginalQuantity")]
        public string MaterialCompOriginalQuantity { get; set; }

        [JsonProperty("EntryUnit")]
        public string EntryUnit { get; set; }

        [JsonProperty("EntryUnitISOCode")]
        public string EntryUnitISOCode { get; set; }

        [JsonProperty("EntryUnitSAPCode")]
        public string EntryUnitSAPCode { get; set; }

        [JsonProperty("GoodsMovementEntryQty")]
        public string GoodsMovementEntryQty { get; set; }

        [JsonProperty("Currency")]
        public string Currency { get; set; }

        [JsonProperty("WithdrawnQuantityAmount")]
        public string WithdrawnQuantityAmount { get; set; }

        [JsonProperty("LastChangeDateTime")]
        public string LastChangeDateTime { get; set; }
    }

}
