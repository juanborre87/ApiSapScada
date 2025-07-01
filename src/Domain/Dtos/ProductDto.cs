using Newtonsoft.Json;

namespace Domain.Dtos
{
    public class ProductDto
    {
        [JsonProperty("Product")]
        public string Product { get; set; }

        [JsonProperty("ProductType")]
        public string ProductType { get; set; }

        [JsonProperty("CrossPlantStatus")]
        public string CrossPlantStatus { get; set; }

        [JsonProperty("CrossPlantStatusValidityDate")]
        public string CrossPlantStatusValidityDate { get; set; }

        [JsonProperty("CreationDate")]
        public string CreationDate { get; set; }

        [JsonProperty("CreatedByUser")]
        public string CreatedByUser { get; set; }

        [JsonProperty("LastChangeDate")]
        public string LastChangeDate { get; set; }

        [JsonProperty("LastChangedByUser")]
        public string LastChangedByUser { get; set; }

        [JsonProperty("LastChangeDateTime")]
        public string LastChangeDateTime { get; set; }

        [JsonProperty("IsMarkedForDeletion")]
        public bool IsMarkedForDeletion { get; set; }

        [JsonProperty("ProductOldID")]
        public string ProductOldID { get; set; }

        [JsonProperty("GrossWeight")]
        public string GrossWeight { get; set; }

        [JsonProperty("PurchaseOrderQuantityUnit")]
        public string PurchaseOrderQuantityUnit { get; set; }

        [JsonProperty("SourceOfSupply")]
        public string SourceOfSupply { get; set; }

        [JsonProperty("WeightUnit")]
        public string WeightUnit { get; set; }

        [JsonProperty("NetWeight")]
        public string NetWeight { get; set; }

        [JsonProperty("CountryOfOrigin")]
        public string CountryOfOrigin { get; set; }

        [JsonProperty("CompetitorID")]
        public string CompetitorID { get; set; }

        [JsonProperty("ProductGroup")]
        public string ProductGroup { get; set; }

        [JsonProperty("BaseUnit")]
        public string BaseUnit { get; set; }

        [JsonProperty("ItemCategoryGroup")]
        public string ItemCategoryGroup { get; set; }

        [JsonProperty("ProductHierarchy")]
        public string ProductHierarchy { get; set; }

        [JsonProperty("Division")]
        public string Division { get; set; }

        [JsonProperty("VarblPurOrdUnitIsActive")]
        public string VarblPurOrdUnitIsActive { get; set; }

        [JsonProperty("VolumeUnit")]
        public string VolumeUnit { get; set; }

        [JsonProperty("MaterialVolume")]
        public string MaterialVolume { get; set; }

        [JsonProperty("ANPCode")]
        public string ANPCode { get; set; }

        [JsonProperty("Brand")]
        public string Brand { get; set; }

        [JsonProperty("ProcurementRule")]
        public string ProcurementRule { get; set; }

        [JsonProperty("ValidityStartDate")]
        public string ValidityStartDate { get; set; }

        [JsonProperty("LowLevelCode")]
        public string LowLevelCode { get; set; }

        [JsonProperty("ProdNoInGenProdInPrepackProd")]
        public string ProdNoInGenProdInPrepackProd { get; set; }

        [JsonProperty("SerialIdentifierAssgmtProfile")]
        public string SerialIdentifierAssgmtProfile { get; set; }

        [JsonProperty("SizeOrDimensionText")]
        public string SizeOrDimensionText { get; set; }

        [JsonProperty("IndustryStandardName")]
        public string IndustryStandardName { get; set; }

        [JsonProperty("ProductStandardID")]
        public string ProductStandardID { get; set; }

        [JsonProperty("InternationalArticleNumberCat")]
        public string InternationalArticleNumberCat { get; set; }

        [JsonProperty("ProductIsConfigurable")]
        public bool ProductIsConfigurable { get; set; }

        [JsonProperty("IsBatchManagementRequired")]
        public bool IsBatchManagementRequired { get; set; }

        [JsonProperty("ExternalProductGroup")]
        public string ExternalProductGroup { get; set; }

        [JsonProperty("CrossPlantConfigurableProduct")]
        public string CrossPlantConfigurableProduct { get; set; }

        [JsonProperty("SerialNoExplicitnessLevel")]
        public string SerialNoExplicitnessLevel { get; set; }

        [JsonProperty("ProductManufacturerNumber")]
        public string ProductManufacturerNumber { get; set; }

        [JsonProperty("ManufacturerNumber")]
        public string ManufacturerNumber { get; set; }

        [JsonProperty("ManufacturerPartProfile")]
        public string ManufacturerPartProfile { get; set; }

        [JsonProperty("QltyMgmtInProcmtIsActive")]
        public bool QltyMgmtInProcmtIsActive { get; set; }

        [JsonProperty("IndustrySector")]
        public string IndustrySector { get; set; }

        [JsonProperty("ChangeNumber")]
        public string ChangeNumber { get; set; }

        [JsonProperty("MaterialRevisionLevel")]
        public string MaterialRevisionLevel { get; set; }

        [JsonProperty("HandlingIndicator")]
        public string HandlingIndicator { get; set; }

        [JsonProperty("WarehouseProductGroup")]
        public string WarehouseProductGroup { get; set; }

        [JsonProperty("WarehouseStorageCondition")]
        public string WarehouseStorageCondition { get; set; }

        [JsonProperty("StandardHandlingUnitType")]
        public string StandardHandlingUnitType { get; set; }

        [JsonProperty("SerialNumberProfile")]
        public string SerialNumberProfile { get; set; }

        [JsonProperty("AdjustmentProfile")]
        public string AdjustmentProfile { get; set; }

        [JsonProperty("PreferredUnitOfMeasure")]
        public string PreferredUnitOfMeasure { get; set; }

        [JsonProperty("IsPilferable")]
        public bool IsPilferable { get; set; }

        [JsonProperty("IsRelevantForHzdsSubstances")]
        public bool IsRelevantForHzdsSubstances { get; set; }

        [JsonProperty("QuarantinePeriod")]
        public string QuarantinePeriod { get; set; }

        [JsonProperty("TimeUnitForQuarantinePeriod")]
        public string TimeUnitForQuarantinePeriod { get; set; }

        [JsonProperty("QualityInspectionGroup")]
        public string QualityInspectionGroup { get; set; }

        [JsonProperty("AuthorizationGroup")]
        public string AuthorizationGroup { get; set; }

        [JsonProperty("DocumentIsCreatedByCAD")]
        public bool DocumentIsCreatedByCAD { get; set; }

        [JsonProperty("HandlingUnitType")]
        public string HandlingUnitType { get; set; }

        [JsonProperty("HasVariableTareWeight")]
        public bool HasVariableTareWeight { get; set; }

        [JsonProperty("MaximumPackagingLength")]
        public string MaximumPackagingLength { get; set; }

        [JsonProperty("MaximumPackagingWidth")]
        public string MaximumPackagingWidth { get; set; }

        [JsonProperty("MaximumPackagingHeight")]
        public string MaximumPackagingHeight { get; set; }

        [JsonProperty("UnitForMaxPackagingDimensions")]
        public string UnitForMaxPackagingDimensions { get; set; }

        // Relaciones diferidas (opcionalmente podrían ser strings si no se navegan)
        [JsonProperty("to_Description")]
        public DeferredUri ToDescription { get; set; }

        [JsonProperty("to_Plant")]
        public DeferredUri ToPlant { get; set; }

        [JsonProperty("to_ProductBasicText")]
        public DeferredUri ToProductBasicText { get; set; }

        [JsonProperty("to_ProductInspectionText")]
        public DeferredUri ToProductInspectionText { get; set; }

        [JsonProperty("to_ProductProcurement")]
        public DeferredUri ToProductProcurement { get; set; }

        [JsonProperty("to_ProductPurchaseText")]
        public DeferredUri ToProductPurchaseText { get; set; }

        [JsonProperty("to_ProductQualityMgmt")]
        public DeferredUri ToProductQualityMgmt { get; set; }

        [JsonProperty("to_ProductSales")]
        public DeferredUri ToProductSales { get; set; }

        [JsonProperty("to_ProductSalesTax")]
        public DeferredUri ToProductSalesTax { get; set; }

        [JsonProperty("to_ProductStorage")]
        public DeferredUri ToProductStorage { get; set; }

        [JsonProperty("to_ProductUnitsOfMeasure")]
        public DeferredUri ToProductUnitsOfMeasure { get; set; }

        [JsonProperty("to_SalesDelivery")]
        public DeferredUri ToSalesDelivery { get; set; }

        [JsonProperty("to_Valuation")]
        public DeferredUri ToValuation { get; set; }
    }

    public class DeferredUri
    {
        [JsonProperty("__deferred")]
        public UriHolder Deferred { get; set; }
    }

    public class UriHolder
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
