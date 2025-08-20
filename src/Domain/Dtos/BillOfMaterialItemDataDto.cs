using Newtonsoft.Json;

namespace Domain.Dtos;

public class BillOfMaterialItemDataDto
{
    [JsonProperty("results")]
    public List<BillOfMaterialItemDataResultDto> Results { get; set; }
}


public class BillOfMaterialItemDataResultDto
{
    [JsonProperty("BillOfMaterialItemUUID")]
    public string BillOfMaterialItemUUID { get; set; }

    [JsonProperty("BillOfMaterialCategory")]
    public string BillOfMaterialCategory { get; set; }

    [JsonProperty("BillOfMaterial")]
    public string BillOfMaterial { get; set; }

    [JsonProperty("BillOfMaterialVariant")]
    public string BillOfMaterialVariant { get; set; }

    [JsonProperty("BillOfMaterialVersion")]
    public string BillOfMaterialVersion { get; set; }

    [JsonProperty("BillOfMaterialItemNodeNumber")]
    public string BillOfMaterialItemNodeNumber { get; set; }

    [JsonProperty("BOMItemInternalChangeCount")]
    public string BOMItemInternalChangeCount { get; set; }

    [JsonProperty("ValidityStartDate")]
    public string ValidityStartDate { get; set; }

    [JsonProperty("ValidityEndDate")]
    public string ValidityEndDate { get; set; }

    [JsonProperty("EngineeringChangeDocForEdit")]
    public string EngineeringChangeDocForEdit { get; set; }

    [JsonProperty("EngineeringChangeDocument")]
    public string EngineeringChangeDocument { get; set; }

    [JsonProperty("ChgToEngineeringChgDocument")]
    public string ChgToEngineeringChgDocument { get; set; }

    [JsonProperty("InheritedNodeNumberForBOMItem")]
    public string InheritedNodeNumberForBOMItem { get; set; }

    [JsonProperty("BOMItemRecordCreationDate")]
    public string BOMItemRecordCreationDate { get; set; }

    [JsonProperty("BOMItemCreatedByUser")]
    public string BOMItemCreatedByUser { get; set; }

    [JsonProperty("BOMItemLastChangeDate")]
    public string BOMItemLastChangeDate { get; set; }

    [JsonProperty("BOMItemLastChangedByUser")]
    public string BOMItemLastChangedByUser { get; set; }

    [JsonProperty("BillOfMaterialComponent")]
    public string BillOfMaterialComponent { get; set; }

    [JsonProperty("BillOfMaterialItemCategory")]
    public string BillOfMaterialItemCategory { get; set; }

    [JsonProperty("BillOfMaterialItemNumber")]
    public string BillOfMaterialItemNumber { get; set; }

    [JsonProperty("BillOfMaterialItemUnit")]
    public string BillOfMaterialItemUnit { get; set; }

    [JsonProperty("BillOfMaterialItemQuantity")]
    public string BillOfMaterialItemQuantity { get; set; }

    [JsonProperty("IsAssembly")]
    public string IsAssembly { get; set; }

    [JsonProperty("IsSubItem")]
    public bool IsSubItem { get; set; }

    [JsonProperty("FixedQuantity")]
    public string FixedQuantity { get; set; }

    [JsonProperty("MaterialComponentPrice")]
    public string MaterialComponentPrice { get; set; }

    [JsonProperty("IdentifierBOMItem")]
    public string IdentifierBOMItem { get; set; }

    [JsonProperty("MaterialPriceUnitQty")]
    public string MaterialPriceUnitQty { get; set; }

    [JsonProperty("ComponentScrapInPercent")]
    public string ComponentScrapInPercent { get; set; }

    [JsonProperty("OperationScrapInPercent")]
    public string OperationScrapInPercent { get; set; }

    [JsonProperty("IsNetScrap")]
    public bool IsNetScrap { get; set; }

    [JsonProperty("NumberOfVariableSizeItem")]
    public string NumberOfVariableSizeItem { get; set; }

    [JsonProperty("QuantityVariableSizeItem")]
    public string QuantityVariableSizeItem { get; set; }

    [JsonProperty("DependencyObjectNumber")]
    public string DependencyObjectNumber { get; set; }

    [JsonProperty("ObjectType")]
    public string ObjectType { get; set; }

    [JsonProperty("IsClassificationRelevant")]
    public bool IsClassificationRelevant { get; set; }

    [JsonProperty("IsBulkMaterial")]
    public bool IsBulkMaterial { get; set; }

    [JsonProperty("IsProductionRelevant")]
    public bool IsProductionRelevant { get; set; }

    [JsonProperty("BOMItemIsPlantMaintRelevant")]
    public bool BOMItemIsPlantMaintRelevant { get; set; }

    [JsonProperty("IsEngineeringRelevant")]
    public bool IsEngineeringRelevant { get; set; }

    [JsonProperty("IsBOMRecursiveAllowed")]
    public bool IsBOMRecursiveAllowed { get; set; }

    [JsonProperty("BOMIsRecursive")]
    public bool BOMIsRecursive { get; set; }

    [JsonProperty("DocumentIsCreatedByCAD")]
    public bool DocumentIsCreatedByCAD { get; set; }

    [JsonProperty("RequiredComponent")]
    public bool RequiredComponent { get; set; }

    [JsonProperty("MultipleSelectionAllowed")]
    public bool MultipleSelectionAllowed { get; set; }

    [JsonProperty("MaterialIsCoProduct")]
    public bool MaterialIsCoProduct { get; set; }

    [JsonProperty("IsDeleted")]
    public bool IsDeleted { get; set; }

    [JsonProperty("IsALE")]
    public bool IsALE { get; set; }

    [JsonProperty("BillOfMaterialHeaderUUID")]
    public string BillOfMaterialHeaderUUID { get; set; }
}