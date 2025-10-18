using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class Text
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }
}
