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
        public required string Zaaktype { get; set; }

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
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("identificatie")]
        public string Identificatie { get; set; } = string.Empty;

        [JsonPropertyName("omschrijving")]
        public string Omschrijving { get; set; } = string.Empty;

        [JsonPropertyName("zaaktype")]
        public string Zaaktype { get; set; } = string.Empty;

        [JsonPropertyName("bronorganisatie")]
        public string Bronorganisatie { get; set; } = string.Empty;

        [JsonPropertyName("verantwoordelijkeOrganisatie")]
        public string VerantwoordelijkeOrganisatie { get; set; } = string.Empty;

        [JsonPropertyName("registratiedatum")]
        public DateTime Registratiedatum { get; set; }

        [JsonPropertyName("startdatum")]
        public DateTime Startdatum { get; set; }
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

        //{"type":"https://openzaak.dev.kiss-demo.nl/ref/fouten/ValidationError/","code":"invalid","title":"Invalid input.","status":400,
        /// <summary>
        /// /"detail":"","instance":"urn:uuid:505c93c7-0dd0-460d-8e8b-8377a19e0c46",
        /// "invalidParams":[{"name":"zaaktype","code":"unknown-service","reason":"De service voor deze URL is niet bekend."}]}
        /// </summary>



        [JsonPropertyName("detail")]
        public string? Detail { get; set; }

        [JsonPropertyName("invalidParams")]
        public List<OzValidationError>? InvalidParams { get; set; }
    }
}
