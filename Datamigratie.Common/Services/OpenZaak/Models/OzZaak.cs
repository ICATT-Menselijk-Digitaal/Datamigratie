using System.Text.Json.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class CreateOzZaakRequest
    {
        [JsonPropertyName("identificatie")]
        public string? Identificatie { get; set; }

        [JsonPropertyName("bronorganisatie")]
        public required string Bronorganisatie { get; set; }

        [JsonPropertyName("omschrijving")]
        public string? Omschrijving { get; set; }

        [JsonPropertyName("zaaktype")]
        public required Uri Zaaktype { get; set; }

        [JsonPropertyName("registratiedatum")]
        public string? Registratiedatum { get; set; }

        [JsonPropertyName("verantwoordelijkeOrganisatie")]
        public required string VerantwoordelijkeOrganisatie { get; set; }

        [JsonPropertyName("startdatum")]
        public required string Startdatum { get; set; }

        [JsonPropertyName("vertrouwelijkheidaanduiding")]
        public string? Vertrouwelijkheidaanduiding { get; set; }

        [JsonPropertyName("betalingsindicatie")]
        public string? Betalingsindicatie { get; set; }

        [JsonPropertyName("archiefstatus")]
        public string? Archiefstatus { get; set; }
    }

    public class OzZaak
    {
        [JsonPropertyName("url")]
        public required Uri Url { get; set; }

        [JsonPropertyName("identificatie")]
        public string Identificatie { get; set; } = string.Empty;

        [JsonPropertyName("omschrijving")]
        public string Omschrijving { get; set; } = string.Empty;

        [JsonPropertyName("zaaktype")]
        public required Uri Zaaktype { get; set; }

        [JsonPropertyName("bronorganisatie")]
        public string Bronorganisatie { get; set; } = string.Empty;

        [JsonPropertyName("verantwoordelijkeOrganisatie")]
        public string VerantwoordelijkeOrganisatie { get; set; } = string.Empty;

        [JsonPropertyName("registratiedatum")]
        public DateOnly Registratiedatum { get; set; }

        [JsonPropertyName("startdatum")]
        public DateOnly Startdatum { get; set; }
    }

    public class OzValidationError
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;
    }

    public class OzErrorResponse
    {

        [JsonPropertyName("detail")]
        public string? Detail { get; set; }

        [JsonPropertyName("invalidParams")]
        public List<OzValidationError>? InvalidParams { get; set; }
    }
}
