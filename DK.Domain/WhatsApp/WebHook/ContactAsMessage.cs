using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class ContactAsMessage
    {
        [JsonPropertyName("addresses")]
        public List<Address>? Addresses { get; set; }

        [JsonPropertyName("birthday")]
        public string? Birthday { get; set; }

        [JsonPropertyName("emails")]
        public List<Email>? Emails { get; set; }

        [JsonPropertyName("name")]
        public Name? Name { get; set; }

        [JsonPropertyName("org")]
        public Organization? Org { get; set; }

        [JsonPropertyName("phones")]
        public List<Phone>? Phones { get; set; }

        [JsonPropertyName("urls")]
        public List<Url>? Urls { get; set; }

    }
}
