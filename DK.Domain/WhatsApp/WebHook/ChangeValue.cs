using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DK.Domain.WhatsApp.WebHook
{
    public class ChangeValue
    {
        [JsonPropertyName("messaging_product")]
        public string? MessagingProduct { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata? Metadata { get; set; }

        [JsonPropertyName("contacts")]
        public List<Contact>? Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<Message>? Messages { get; set; }

    }
}
