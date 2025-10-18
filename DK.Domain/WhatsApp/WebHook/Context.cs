using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class Context
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("referred_product")]
        public ReferredProduct? ReferredProduct { get; set; }

    }
}
