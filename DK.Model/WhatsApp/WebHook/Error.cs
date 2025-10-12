using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Error
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("details")]
        public string? Details { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("error_data")]
        public ErrorData? ErrorData { get; set; }

        [JsonPropertyName("href")]
        public string? Href { get; set; }


    }
}
