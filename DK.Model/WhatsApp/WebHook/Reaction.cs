using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Reaction
    {
        [JsonPropertyName("message_id")]
        public string? MessageId { get; set; }

        [JsonPropertyName("emoji")]
        public string? Emoji { get; set; }
    }
}
