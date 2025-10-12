using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Name
    {
        [JsonPropertyName("formatted_name")]
        public string? FormattedName { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("middle_name")]
        public string? MiddleName { get; set; }

        [JsonPropertyName("suffix")]
        public string? Suffix { get; set; }

        [JsonPropertyName("prefix")]
        public string? Prefix { get; set; }


    }
}
