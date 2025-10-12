using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class System
    {
        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("new_wa_id")]
        public string? NewWaId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
