using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DK.Model.WhatsApp.WebHook
{
    public class Event
    {
        [JsonPropertyName("entry")]
        public List<Entry>? Entry { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }
    }
}
