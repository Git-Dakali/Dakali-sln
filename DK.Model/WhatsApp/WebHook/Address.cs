using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Address
    {
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("zip")]
        public string? Zip { get; set; }


    }
}
