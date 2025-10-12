using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Image
    {
        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }

        [JsonPropertyName("sha256")]
        public string? Sha256 { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
