using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class Entry
    {
        [JsonPropertyName("Time")]
        public long? Time { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("changes")]
        public List<Change>? Changes { get; set; }
    }
}
