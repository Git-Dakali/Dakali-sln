using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class Organization
    {
        [JsonPropertyName("company")]
        public string? Company { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
