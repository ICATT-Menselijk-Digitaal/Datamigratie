using System.Text.Json.Serialization;
using Datamigratie.Common.Helpers;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class OzInformatieobjecttype
    {
        private string _url = string.Empty;

        [JsonPropertyName("url")]
        public required string Url
        {
            get => _url;
            set
            {
                _url = value;
                if (!string.IsNullOrEmpty(value))
                {
                    Id = OzUrlToGuidConverter.ExtractUuidFromUrl(value);
                }
            }
        }

        public Guid Id { get; private set; } = Guid.Empty;

        [JsonPropertyName("omschrijving")]
        public required string Omschrijving { get; set; }

        [JsonPropertyName("catalogus")]
        public string? Catalogus { get; set; }

        [JsonPropertyName("vertrouwelijkheidaanduiding")]
        public string? Vertrouwelijkheidaanduiding { get; set; }
    }
}
