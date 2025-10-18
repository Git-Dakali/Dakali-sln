using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class Change
    {
        [JsonPropertyName("field")]
        public string? field { get; set; }

        [JsonPropertyName("value")]
        public ChangeValue? Value { get; set; }
    }
}
