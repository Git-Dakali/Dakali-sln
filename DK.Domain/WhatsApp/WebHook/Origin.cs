using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class Origin
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
