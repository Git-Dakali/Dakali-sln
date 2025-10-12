using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class ErrorData
    {
        [JsonPropertyName("details")]
        public string? Details { get; set; }
    }
}
