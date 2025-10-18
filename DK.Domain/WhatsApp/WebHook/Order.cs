using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class Order
    {
        [JsonPropertyName("catalog_id")]
        public string? CatalogId { get; set; }

        [JsonPropertyName("product_items")]
        public List<ProductItem>? ProductItems { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

}
