using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class ButtonReply
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
