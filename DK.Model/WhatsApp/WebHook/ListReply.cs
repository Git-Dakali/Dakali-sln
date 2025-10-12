using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class ListReply
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

    }

}
