using Newtonsoft.Json;

namespace Domain.Dtos;

public class MasterRecipeMatlAssgmtDto
{
    [JsonProperty("results")]
    public List<MasterRecipeMatlAssgmtResultDto> Results { get; set; }
}

public class MasterRecipeMatlAssgmtResultDto
{

    [JsonProperty("Product")]
    public string Product { get; set; }

    [JsonProperty("Plant")]
    public string Plant { get; set; }

    [JsonProperty("MasterRecipeGroup")]
    public string MasterRecipeGroup { get; set; }

    [JsonProperty("MasterRecipe")]
    public string MasterRecipe { get; set; }

    [JsonProperty("MasterRecipeMaterialAssignment")]
    public string MasterRecipeMaterialAssignment { get; set; }

    [JsonProperty("MstrRcpMatlAssgmtIntVersion")]
    public string MstrRcpMatlAssgmtIntVersion { get; set; }

    [JsonProperty("CreationDate")]
    public string CreationDate { get; set; }

    [JsonProperty("CreatedByUser")]
    public string CreatedByUser { get; set; }

    [JsonProperty("LastChangeDate")]
    public string LastChangeDate { get; set; }

    [JsonProperty("LastChangedByUser")]
    public string LastChangedByUser { get; set; }

    [JsonProperty("ValidityStartDate")]
    public string ValidityStartDate { get; set; }

    [JsonProperty("ValidityEndDate")]
    public string ValidityEndDate { get; set; }

    [JsonProperty("ChangeNumber")]
    public string ChangeNumber { get; set; }

    [JsonProperty("ChangedDateTime")]
    public string ChangedDateTime { get; set; }
}
