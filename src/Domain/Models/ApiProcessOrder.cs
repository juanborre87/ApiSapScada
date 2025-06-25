namespace Domain.Models
{
    public class ApiProcessOrder
    {
        public ProcessOrder d { get; set; }
    }

    public class ProcessOrder
    {
        public ProcessOrderMetadata __metadata { get; set; }
        public string ManufacturingOrder { get; set; }
        public string ManufacturingOrderCategory { get; set; }
        public string ManufacturingOrderType { get; set; }
        public string OrderLongText { get; set; }
        public string ManufacturingOrderImportance { get; set; }
        public string OrderIsCreated { get; set; }
        public string OrderIsReleased { get; set; }
        public string OrderIsPrinted { get; set; }
        public string OrderIsConfirmed { get; set; }
        public string OrderIsPartiallyConfirmed { get; set; }
        public string OrderIsDelivered { get; set; }
        public string OrderIsDeleted { get; set; }
        public string OrderIsPreCosted { get; set; }
        public string SettlementRuleIsCreated { get; set; }
        public string OrderIsPartiallyReleased { get; set; }
        public string OrderIsLocked { get; set; }
        public string OrderIsTechnicallyCompleted { get; set; }
        public string OrderIsClosed { get; set; }
        public string OrderIsPartiallyDelivered { get; set; }
        public string OrderIsMarkedForDeletion { get; set; }
        public string SettlementRuleIsCrtedManually { get; set; }
        public string OrderIsScheduled { get; set; }
        public string OrderHasGeneratedOperations { get; set; }
        public string OrderIsToBeHandledInBatches { get; set; }
        public string MaterialAvailyIsNotChecked { get; set; }
        public string MfgOrderCreationDate { get; set; }
        public string MfgOrderCreationTime { get; set; }
        public string LastChangeDateTime { get; set; }
        public string Material { get; set; }
        public string StorageLocation { get; set; }
        public string GoodsRecipientName { get; set; }
        public string UnloadingPointName { get; set; }
        public string InventoryUsabilityCode { get; set; }
        public string MaterialGoodsReceiptDuration { get; set; }
        public string QuantityDistributionKey { get; set; }
        public string StockSegment { get; set; }
        public string OrderInternalBillOfOperations { get; set; }
        public string ProductionPlant { get; set; }
        public string Plant { get; set; }
        public string MRPArea { get; set; }
        public string MRPController { get; set; }
        public string ProductionSupervisor { get; set; }
        public string ProductionVersion { get; set; }
        public string PlannedOrder { get; set; }
        public string SalesOrder { get; set; }
        public string SalesOrderItem { get; set; }
        public string BasicSchedulingType { get; set; }
        public string ManufacturingObject { get; set; }
        public string ProductConfiguration { get; set; }
        public string OrderSequenceNumber { get; set; }
        public string BusinessArea { get; set; }
        public string CompanyCode { get; set; }
        public string ProfitCenter { get; set; }
        public string ActualCostsCostingVariant { get; set; }
        public string PlannedCostsCostingVariant { get; set; }
        public string FunctionalArea { get; set; }
        public string MfgOrderPlannedStartDate { get; set; }
        public string MfgOrderPlannedStartTime { get; set; }
        public string MfgOrderPlannedEndDate { get; set; }
        public string MfgOrderPlannedEndTime { get; set; }
        public string MfgOrderScheduledStartDate { get; set; }
        public string MfgOrderScheduledStartTime { get; set; }
        public string MfgOrderScheduledEndDate { get; set; }
        public string MfgOrderScheduledEndTime { get; set; }
        public object MfgOrderActualReleaseDate { get; set; }
        public string ProductionUnit { get; set; }
        public string ProductionUnitISOCode { get; set; }
        public string ProductionUnitSAPCode { get; set; }
        public string TotalQuantity { get; set; }
        public string MfgOrderPlannedScrapQty { get; set; }
        public string MfgOrderConfirmedYieldQty { get; set; }
        public string CustomerName { get; set; }
        public string WBSElementExternalID { get; set; }
        public ToDeferred to_ProcessOrderComponent { get; set; }
        public ToDeferred to_ProcessOrderItem { get; set; }
        public ToDeferred to_ProcessOrderOperation { get; set; }
        public ToDeferred to_ProcessOrderStatus { get; set; }
        public ToDeferred to_ProcessProdnRsceTools { get; set; }
    }

    public class ProcessOrderMetadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
        public string etag { get; set; }
    }

    public class ToDeferred
    {
        public Deferred __deferred { get; set; }
    }

    public class Deferred
    {
        public string uri { get; set; }
    }
}
