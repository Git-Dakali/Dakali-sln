using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Message
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("timestamp")] 
        public string? Timestamp { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public Text? Text { get; set; }

        [JsonPropertyName("reaction")]
        public Reaction? Reaction { get; set; }

        [JsonPropertyName("image")]
        public Image? Image { get; set; }

        [JsonPropertyName("sticker")]
        public Sticker? Sticker { get; set; }

        [JsonPropertyName("errors")]
        public List<Error>? Errors { get; set; }

        [JsonPropertyName("button")]
        public Button? Button { get; set; }

        [JsonPropertyName("location")]
        public Location? Location { get; set; }

        [JsonPropertyName("contacts")]
        public List<ContactAsMessage>? Contacts { get; set; }

        [JsonPropertyName("interactive")]
        public Interactive? Interactive { get; set; }

        [JsonPropertyName("referral")]
        public Referral? Referral { get; set; }

        [JsonPropertyName("context")]
        public Context? Context { get; set; }

        [JsonPropertyName("order")]
        public Order? Order { get; set; }

        [JsonPropertyName("system")]
        public System? System { get; set; }

        [JsonPropertyName("statuses")]
        public List<Status>? Statuses { get; set; }
    }
}
