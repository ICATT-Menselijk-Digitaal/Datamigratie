using System.Text.Json.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class CreateOzStatusRequest
    {
        [JsonPropertyName("zaak")]
        public required Uri Zaak { get; set; }

        [JsonPropertyName("statustype")]
        public required Uri Statustype { get; set; }

        [JsonPropertyName("datumStatusGezet")]
        public required DateTime DatumStatusGezet { get; set; }

        [JsonPropertyName("statustoelichting")]
        public string? Statustoelichting { get; set; }
    }

    public class OzStatus
    {
        [JsonPropertyName("url")]
        public required Uri Url { get; set; }

        [JsonPropertyName("zaak")]
        public required Uri Zaak { get; set; }

        [JsonPropertyName("statustype")]
        public required Uri Statustype { get; set; }

        [JsonPropertyName("datumStatusGezet")]
        public required DateTime DatumStatusGezet { get; set; }

        [JsonPropertyName("statustoelichting")]
        public string? Statustoelichting { get; set; }
    }
}
