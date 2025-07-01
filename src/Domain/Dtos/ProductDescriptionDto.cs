using Newtonsoft.Json;

namespace Domain.Dtos
{
    public class ProductDescriptionDto
    {
        [JsonProperty("results")]
        public List<ProductDescriptionResultDto> Results { get; set; }

    }

    public class ProductDescriptionResultDto
    {
        [JsonProperty("Product")]
        public string Product { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("ProductDescription")]
        public string ProductDescription { get; set; }
    }
}
