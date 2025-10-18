using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class Location
    {
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

    }
}
