using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class ProductItem
    {
        [JsonPropertyName("product_retailer_id")]
        public string? ProductRetailerId { get; set; }

        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }

        [JsonPropertyName("item_price")]
        public decimal? ItemPrice { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }

}
