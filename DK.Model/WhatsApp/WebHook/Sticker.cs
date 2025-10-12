using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Sticker
    {
        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }

        [JsonPropertyName("sha256")]
        public string? Sha256 { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
