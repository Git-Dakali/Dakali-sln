using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Interactive
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("list_reply")]
        public ListReply? ListReply { get; set; }

        [JsonPropertyName("button_reply")]
        public ButtonReply? ButtonReply { get; set; }
    }
}
