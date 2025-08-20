using Newtonsoft.Json;

namespace Domain.Dtos
{
    public class BillOfMaterialHeaderDto
    {
        [JsonProperty("results")]
        public List<BillOfMaterialHeaderResultDto> Results { get; set; }
    }

    public class BillOfMaterialHeaderResultDto
    {

        [JsonProperty("BillOfMaterialHeaderUUID")]
        public string BillOfMaterialHeaderUUID { get; set; }

        [JsonProperty("BillOfMaterialVariantUsage")]
        public string BillOfMaterialVariantUsage { get; set; }

        [JsonProperty("BillOfMaterialCategory")]
        public string BillOfMaterialCategory { get; set; }

        [JsonProperty("BillOfMaterial")]
        public string BillOfMaterial { get; set; }

        [JsonProperty("BillOfMaterialVariant")]
        public string BillOfMaterialVariant { get; set; }

        [JsonProperty("Material")]
        public string Material { get; set; }

        [JsonProperty("Plant")]
        public string Plant { get; set; }

        [JsonProperty("IsMultipleBOMAlt")]
        public bool IsMultipleBOMAlt { get; set; }

        [JsonProperty("BOMHeaderInternalChangeCount")]
        public string BOMHeaderInternalChangeCount { get; set; }

        [JsonProperty("BOMUsagePriority")]
        public string BOMUsagePriority { get; set; }

        [JsonProperty("BillOfMaterialAuthsnGrp")]
        public string BillOfMaterialAuthsnGrp { get; set; }

        [JsonProperty("BillOfMaterialVersion")]
        public string BillOfMaterialVersion { get; set; }

        [JsonProperty("BOMVersionStatus")]
        public string BOMVersionStatus { get; set; }

        [JsonProperty("IsVersionBillOfMaterial")]
        public bool IsVersionBillOfMaterial { get; set; }

        [JsonProperty("IsLatestBOMVersion")]
        public bool IsLatestBOMVersion { get; set; }

        [JsonProperty("IsConfiguredMaterial")]
        public bool IsConfiguredMaterial { get; set; }

        [JsonProperty("BOMTechnicalType")]
        public string BOMTechnicalType { get; set; }

        [JsonProperty("BOMGroup")]
        public string BOMGroup { get; set; }

        [JsonProperty("BOMHeaderText")]
        public string BOMHeaderText { get; set; }

        [JsonProperty("BOMAlternativeText")]
        public string BOMAlternativeText { get; set; }

        [JsonProperty("BillOfMaterialStatus")]
        public string BillOfMaterialStatus { get; set; }

        [JsonProperty("HeaderValidityStartDate")]
        public string HeaderValidityStartDate { get; set; }

        [JsonProperty("HeaderValidityEndDate")]
        public string HeaderValidityEndDate { get; set; }

        [JsonProperty("EngineeringChangeDocument")]
        public string EngineeringChangeDocument { get; set; }

        [JsonProperty("EngineeringChangeDocForEdit")]
        public string EngineeringChangeDocForEdit { get; set; }

        [JsonProperty("ChgToEngineeringChgDocument")]
        public string ChgToEngineeringChgDocument { get; set; }

        [JsonProperty("IsMarkedForDeletion")]
        public bool IsMarkedForDeletion { get; set; }

        [JsonProperty("IsALE")]
        public bool IsALE { get; set; }

        [JsonProperty("BOMHeaderBaseUnit")]
        public string BOMHeaderBaseUnit { get; set; }

        [JsonProperty("BOMHeaderQuantityInBaseUnit")]
        public string BOMHeaderQuantityInBaseUnit { get; set; }

        [JsonProperty("RecordCreationDate")]
        public string RecordCreationDate { get; set; }

        [JsonProperty("LastChangeDate")]
        public string LastChangeDate { get; set; }

        [JsonProperty("CreatedByUser")]
        public string CreatedByUser { get; set; }

        [JsonProperty("LastChangedByUser")]
        public string LastChangedByUser { get; set; }

        [JsonProperty("BOMIsToBeDeleted")]
        public string BOMIsToBeDeleted { get; set; }

        [JsonProperty("DocumentIsCreatedByCAD")]
        public bool DocumentIsCreatedByCAD { get; set; }

        [JsonProperty("LaboratoryOrDesignOffice")]
        public string LaboratoryOrDesignOffice { get; set; }

        [JsonProperty("SelectedBillOfMaterialVersion")]
        public string SelectedBillOfMaterialVersion { get; set; }

    }

}
