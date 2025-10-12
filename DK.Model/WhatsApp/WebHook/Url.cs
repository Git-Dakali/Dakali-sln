using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Url
    {
        [JsonPropertyName("url")]
        public string Link { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
