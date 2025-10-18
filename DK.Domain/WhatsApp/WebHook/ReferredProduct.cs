using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class ReferredProduct
    {
        [JsonPropertyName("catalog_id")]
        public string? CatalogId { get; set; }

        [JsonPropertyName("product_retailer_id")]
        public string? ProductRetailerId { get; set; }

    }
}
