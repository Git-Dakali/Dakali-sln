using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Status
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("status")]
        public string? StatusValue { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("recipient_id")]
        public string? RecipientId { get; set; }

        [JsonPropertyName("conversation")]
        public Conversation? Conversation { get; set; }

        [JsonPropertyName("pricing")]
        public Pricing? Pricing { get; set; }

        [JsonPropertyName("errors")]
        public List<Error>? Errors { get; set; }

    }
}
