using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class Email
    {
        [JsonPropertyName("email")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
