using Newtonsoft.Json;

namespace Domain.Models
{
    public class OrderDto
    {
        [JsonProperty("ManufacturingOrder")]
        public string ManufacturingOrder { get; set; }

        [JsonProperty("ManufacturingOrderCategory")]
        public string ManufacturingOrderCategory { get; set; }

        [JsonProperty("ManufacturingOrderType")]
        public string ManufacturingOrderType { get; set; }

        [JsonProperty("OrderLongText")]
        public string OrderLongText { get; set; }

        [JsonProperty("ManufacturingOrderImportance")]
        public string ManufacturingOrderImportance { get; set; }

        [JsonProperty("OrderIsCreated")]
        public string OrderIsCreated { get; set; }

        [JsonProperty("OrderIsReleased")]
        public string OrderIsReleased { get; set; }

        [JsonProperty("OrderIsPrinted")]
        public string OrderIsPrinted { get; set; }

        [JsonProperty("OrderIsConfirmed")]
        public string OrderIsConfirmed { get; set; }

        [JsonProperty("OrderIsPartiallyConfirmed")]
        public string OrderIsPartiallyConfirmed { get; set; }

        [JsonProperty("OrderIsDelivered")]
        public string OrderIsDelivered { get; set; }

        [JsonProperty("OrderIsDeleted")]
        public string OrderIsDeleted { get; set; }

        [JsonProperty("OrderIsPreCosted")]
        public string OrderIsPreCosted { get; set; }

        [JsonProperty("SettlementRuleIsCreated")]
        public string SettlementRuleIsCreated { get; set; }

        [JsonProperty("OrderIsPartiallyReleased")]
        public string OrderIsPartiallyReleased { get; set; }

        [JsonProperty("OrderIsLocked")]
        public string OrderIsLocked { get; set; }

        [JsonProperty("OrderIsTechnicallyCompleted")]
        public string OrderIsTechnicallyCompleted { get; set; }

        [JsonProperty("OrderIsClosed")]
        public string OrderIsClosed { get; set; }

        [JsonProperty("OrderIsPartiallyDelivered")]
        public string OrderIsPartiallyDelivered { get; set; }

        [JsonProperty("OrderIsMarkedForDeletion")]
        public string OrderIsMarkedForDeletion { get; set; }

        [JsonProperty("SettlementRuleIsCrtedManually")]
        public string SettlementRuleIsCrtedManually { get; set; }

        [JsonProperty("OrderIsScheduled")]
        public string OrderIsScheduled { get; set; }

        [JsonProperty("OrderHasGeneratedOperations")]
        public string OrderHasGeneratedOperations { get; set; }

        [JsonProperty("OrderIsToBeHandledInBatches")]
        public string OrderIsToBeHandledInBatches { get; set; }

        [JsonProperty("MaterialAvailyIsNotChecked")]
        public string MaterialAvailyIsNotChecked { get; set; }

        [JsonProperty("MfgOrderCreationDate")]
        public string MfgOrderCreationDate { get; set; }

        [JsonProperty("MfgOrderCreationTime")]
        public string MfgOrderCreationTime { get; set; }

        [JsonProperty("LastChangeDateTime")]
        public string LastChangeDateTime { get; set; }

        [JsonProperty("Material")]
        public string Material { get; set; }

        [JsonProperty("StorageLocation")]
        public string StorageLocation { get; set; }

        [JsonProperty("GoodsRecipientName")]
        public string GoodsRecipientName { get; set; }

        [JsonProperty("UnloadingPointName")]
        public string UnloadingPointName { get; set; }

        [JsonProperty("InventoryUsabilityCode")]
        public string InventoryUsabilityCode { get; set; }

        [JsonProperty("MaterialGoodsReceiptDuration")]
        public string MaterialGoodsReceiptDuration { get; set; }

        [JsonProperty("QuantityDistributionKey")]
        public string QuantityDistributionKey { get; set; }

        [JsonProperty("StockSegment")]
        public string StockSegment { get; set; }

        [JsonProperty("OrderInternalBillOfOperations")]
        public string OrderInternalBillOfOperations { get; set; }

        [JsonProperty("ProductionPlant")]
        public string ProductionPlant { get; set; }

        [JsonProperty("Plant")]
        public string Plant { get; set; }

        [JsonProperty("MRPArea")]
        public string MRPArea { get; set; }

        [JsonProperty("MRPController")]
        public string MRPController { get; set; }

        [JsonProperty("ProductionSupervisor")]
        public string ProductionSupervisor { get; set; }

        [JsonProperty("ProductionVersion")]
        public string ProductionVersion { get; set; }

        [JsonProperty("PlannedOrder")]
        public string PlannedOrder { get; set; }

        [JsonProperty("SalesOrder")]
        public string SalesOrder { get; set; }

        [JsonProperty("SalesOrderItem")]
        public string SalesOrderItem { get; set; }

        [JsonProperty("BasicSchedulingType")]
        public string BasicSchedulingType { get; set; }

        [JsonProperty("ManufacturingObject")]
        public string ManufacturingObject { get; set; }

        [JsonProperty("ProductConfiguration")]
        public string ProductConfiguration { get; set; }

        [JsonProperty("OrderSequenceNumber")]
        public string OrderSequenceNumber { get; set; }

        [JsonProperty("BusinessArea")]
        public string BusinessArea { get; set; }

        [JsonProperty("CompanyCode")]
        public string CompanyCode { get; set; }

        [JsonProperty("ProfitCenter")]
        public string ProfitCenter { get; set; }

        [JsonProperty("ActualCostsCostingVariant")]
        public string ActualCostsCostingVariant { get; set; }

        [JsonProperty("PlannedCostsCostingVariant")]
        public string PlannedCostsCostingVariant { get; set; }

        [JsonProperty("FunctionalArea")]
        public string FunctionalArea { get; set; }

        [JsonProperty("MfgOrderPlannedStartDate")]
        public string MfgOrderPlannedStartDate { get; set; }

        [JsonProperty("MfgOrderPlannedStartTime")]
        public string MfgOrderPlannedStartTime { get; set; }

        [JsonProperty("MfgOrderPlannedEndDate")]
        public string MfgOrderPlannedEndDate { get; set; }

        [JsonProperty("MfgOrderPlannedEndTime")]
        public string MfgOrderPlannedEndTime { get; set; }

        [JsonProperty("MfgOrderScheduledStartDate")]
        public string MfgOrderScheduledStartDate { get; set; }

        [JsonProperty("MfgOrderScheduledStartTime")]
        public string MfgOrderScheduledStartTime { get; set; }

        [JsonProperty("MfgOrderScheduledEndDate")]
        public string MfgOrderScheduledEndDate { get; set; }

        [JsonProperty("MfgOrderScheduledEndTime")]
        public string MfgOrderScheduledEndTime { get; set; }

        [JsonProperty("MfgOrderActualReleaseDate")]
        public string MfgOrderActualReleaseDate { get; set; }

        [JsonProperty("ProductionUnit")]
        public string ProductionUnit { get; set; }

        [JsonProperty("ProductionUnitISOCode")]
        public string ProductionUnitISOCode { get; set; }

        [JsonProperty("ProductionUnitSAPCode")]
        public string ProductionUnitSAPCode { get; set; }

        [JsonProperty("TotalQuantity")]
        public string TotalQuantity { get; set; }

        [JsonProperty("MfgOrderPlannedScrapQty")]
        public string MfgOrderPlannedScrapQty { get; set; }

        [JsonProperty("MfgOrderConfirmedYieldQty")]
        public string MfgOrderConfirmedYieldQty { get; set; }

        [JsonProperty("CustomerName")]
        public string CustomerName { get; set; }

        [JsonProperty("WBSElementExternalID")]
        public string WBSElementExternalID { get; set; }

        // Relaciones diferidas
        [JsonProperty("to_ProcessOrderComponent")]
        public DeferredLink To_ProcessOrderComponent { get; set; }

        [JsonProperty("to_ProcessOrderItem")]
        public DeferredLink To_ProcessOrderItem { get; set; }

        [JsonProperty("to_ProcessOrderOperation")]
        public DeferredLink To_ProcessOrderOperation { get; set; }

        [JsonProperty("to_ProcessOrderStatus")]
        public DeferredLink To_ProcessOrderStatus { get; set; }

        [JsonProperty("to_ProcessProdnRsceTools")]
        public DeferredLink To_ProcessProdnRsceTools { get; set; }
    }

    public class DeferredLink
    {
        [JsonProperty("__deferred")]
        public DeferredUri Deferred { get; set; }
    }

    public class DeferredUri
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
