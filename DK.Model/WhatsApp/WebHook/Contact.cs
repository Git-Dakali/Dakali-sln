using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Contact
    {
        [JsonPropertyName("profile")]
        public Profile? Profile { get; set; }

        [JsonPropertyName("wa_id")]
        public string? WaId { get; set; }
    }
}
