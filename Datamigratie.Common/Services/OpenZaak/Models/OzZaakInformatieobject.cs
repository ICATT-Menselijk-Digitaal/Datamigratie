using System.Text.Json.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class OzZaakInformatieobject
    {
        [JsonPropertyName("url")]
        public required string Url { get; set; }

        [JsonPropertyName("zaak")]
        public required string Zaak { get; set; }

        [JsonPropertyName("informatieobject")]
        public required string Informatieobject { get; set; }
    }
}
