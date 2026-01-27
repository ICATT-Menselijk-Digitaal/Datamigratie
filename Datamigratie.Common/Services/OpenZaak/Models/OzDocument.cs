using System.Text.Json.Serialization;
using Datamigratie.Common.Helpers;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public sealed class OzDocument
    {

        public Guid Id { get; private set; } = Guid.Empty;

        private string? _url = null;

        public string? Url
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

        public string? Identificatie { get; init; }

        public required string Bronorganisatie { get; init; }

        public required DateOnly Creatiedatum { get; init; }

        public required string Titel { get; init; }

        // This is a required field in OpenZaak, but this is not documented in the OZ documentation
        public required VertrouwelijkheidsAanduiding Vertrouwelijkheidaanduiding { get; init; }

        public required string Auteur { get; init; }

        // This is a required field in OpenZaak, but this is not documented in the OZ documentation
        public required DocumentStatus Status { get; init; }

        /// MIME type, bijv. application/pdf
        public string? Formaat { get; init; }

        /// Een ISO 639-2/B taalcode waarin de inhoud van het INFORMATIEOBJECT is vastgelegd. Voorbeeld: dut. Zie: https://www.iso.org/standard/4767.html
        public required string Taal { get; init; }

        public int? Versie { get; init; }

        public DateTimeOffset? BeginRegistratie { get; init; }

        public string? Bestandsnaam { get; init; }

        /// Base64-inhoud voor create; komt normaal niet terug op GET
        public string? Inhoud { get; init; }

        public long? Bestandsomvang { get; init; }

        // This is a required field in OpenZaak (can be whitespace), but this is not documented in the OZ documentation
        public required string Link { get; init; }

        // This is a required field in OpenZaak (can be whitespace), but this is not documented in the OZ documentation
        public required string Beschrijving { get; init; }

        // Date-only (YYYY-MM-DD)
        public DateOnly? Ontvangstdatum { get; init; }

        public DateOnly? Verzenddatum { get; init; }

        public bool? IndicatieGebruiksrecht { get; init; }

        // This is a required field in OpenZaak (can be whitespace), but this is not documented in the OZ documentation
        public required string Verschijningsvorm { get; init; }

        public Ondertekening? Ondertekening { get; init; }

        public Integriteit? Integriteit { get; init; }

        /// URL naar Informatieobjecttype (ZTC)
        public required Uri Informatieobjecttype { get; init; }

        public bool? Locked { get; init; }

        public List<Bestandsdeel>? Bestandsdelen { get; init; }

        // This is a required field in OpenZaak (can be empty), but this is not documented in the OZ documentation
        public required List<string> Trefwoorden { get; init; }

        [JsonPropertyName("_expand")]
        public Expand? Expand { get; init; }

        public string? Lock { get; set; }
    }

    public sealed class Ondertekening
    {
        /// Wijze van ondertekening (bv. nat, elektronisch)
        public string? Wijze { get; init; }

        // Date-only (YYYY-MM-DD)
        public DateOnly? Datum { get; init; }
    }

    public sealed class Integriteit
    {
        public string? Algoritme { get; init; }

        public string? Waarde { get; init; }

        // RFC3339 date-time
        public DateTimeOffset? Datum { get; init; }
    }

    public sealed class Expand
    {
        public Informatieobjecttype? Informatieobjecttype { get; init; }
    }

    public sealed class Informatieobjecttype
    {
        public string? Url { get; init; }

        public string? Catalogus { get; init; }

        public string? Omschrijving { get; init; }

        public VertrouwelijkheidsAanduiding? Vertrouwelijkheidaanduiding { get; init; }

        public DateOnly? BeginGeldigheid { get; init; }

        public DateOnly? EindeGeldigheid { get; init; }

        public DateOnly? BeginObject { get; init; }

        public DateOnly? EindeObject { get; init; }

        public bool? Concept { get; init; }

        public List<string>? Zaaktypen { get; init; }

        public List<string>? Besluittypen { get; init; }

        public string? Informatieobjectcategorie { get; init; }

        public List<string>? Trefwoord { get; init; }

        public OmschrijvingGeneriek? OmschrijvingGeneriek { get; init; }
    }

    public sealed class OmschrijvingGeneriek
    {
        public string? InformatieobjecttypeOmschrijvingGeneriek { get; init; }

        public string? DefinitieInformatieobjecttypeOmschrijvingGeneriek { get; init; }

        public string? HerkomstInformatieobjecttypeOmschrijvingGeneriek { get; init; }

        public string? HierarchieInformatieobjecttypeOmschrijvingGeneriek { get; init; }

        public string? OpmerkingInformatieobjecttypeOmschrijvingGeneriek { get; init; }
    }

    public sealed class Bestandsdeel
    {
        public Uri? Url { get; init; }

        public int? Volgnummer { get; init; }

        public long? Omvang { get; init; }

        public bool? Voltooid { get; init; }

        [JsonPropertyName("_lock")]
        public string? Lock { get; init; }
    }

    // Enums (let op: standaard numeriek zonder JsonStringEnumConverter in web defaults)
    [JsonConverter(typeof(JsonStringEnumConverter<VertrouwelijkheidsAanduiding>))]
    public enum VertrouwelijkheidsAanduiding
    {
        openbaar,
        beperkt_openbaar,
        intern,
        zaakvertrouwelijk,
        vertrouwelijk,
        geheim,
        zeer_geheim
    }

    [JsonConverter(typeof(JsonStringEnumConverter<DocumentStatus>))]
    public enum DocumentStatus
    {
        in_bewerking,
        ter_vaststelling,
        definitief,
        gearchiveerd
    }

}
