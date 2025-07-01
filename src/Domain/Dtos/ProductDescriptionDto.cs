using Newtonsoft.Json;

namespace Domain.Dtos
{
    public class ProductDescriptionDto
    {
        [JsonProperty("Product")]
        public string Product { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("ProductDescription")]
        public string ProductDescription { get; set; }
    }
}
