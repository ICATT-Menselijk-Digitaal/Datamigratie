using System.Text.Json.Serialization;
using Datamigratie.Common.Converters;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class CreateOzBesluitRequest
    {
        /// <summary>
        /// Identificatie van het besluit binnen de organisatie die het besluit heeft vastgesteld.
        /// Indien deze niet opgegeven is, dan wordt die gegenereerd. Max 50 characters.
        /// </summary>
        [JsonPropertyName("identificatie")]
        public string? Identificatie { get; set; }

        /// <summary>
        /// Het RSIN van de niet-natuurlijk persoon zijnde de organisatie die het besluit heeft vastgesteld.
        /// Required, exactly 9 characters.
        /// </summary>
        [JsonPropertyName("verantwoordelijkeOrganisatie")]
        public required string VerantwoordelijkeOrganisatie { get; set; }

        /// <summary>
        /// URL-referentie naar het BESLUITTYPE (in de Catalogi API). Required, 1-200 characters.
        /// </summary>
        [JsonPropertyName("besluittype")]
        public required Uri Besluittype { get; set; }

        /// <summary>
        /// URL-referentie naar de ZAAK (in de Zaken API) waarvan dit besluit uitkomst is. Max 200 characters.
        /// </summary>
        [JsonPropertyName("zaak")]
        public Uri? Zaak { get; set; }

        /// <summary>
        /// De beslisdatum (AWB) van het besluit. Required.
        /// </summary>
        [JsonPropertyName("datum")]
        public required DateOnly Datum { get; set; }

        /// <summary>
        /// Toelichting bij het besluit (is not required in OpenZaak doc, but must be present)
        /// </summary>
        [JsonPropertyName("toelichting")]
        public required string Toelichting { get; set; }

        /// <summary>
        /// Een orgaan van een rechtspersoon krachtens publiekrecht ingesteld of een persoon of college,
        /// met enig openbaar gezag bekleed onder wiens verantwoordelijkheid het besluit vastgesteld is. Max 50 characters.
        /// 
        /// Is not required in OpenZaak doc, but must be present.
        /// </summary>
        [JsonPropertyName("bestuursorgaan")]
        public required string Bestuursorgaan { get; set; }

        /// <summary>
        /// Ingangsdatum van de werkingsperiode van het besluit. Required.
        /// </summary>
        [JsonPropertyName("ingangsdatum")]
        public required DateOnly Ingangsdatum { get; set; }

        /// <summary>
        /// Datum waarop de werkingsperiode van het besluit eindigt.
        /// </summary>
        [JsonPropertyName("vervaldatum")]
        public DateOnly? Vervaldatum { get; set; }

        /// <summary>
        /// De omschrijving die aangeeft op grond waarvan het besluit is of komt te vervallen.
        /// </summary>
        [JsonPropertyName("vervalreden")]
        public Vervalreden? Vervalreden { get; set; }

        /// <summary>
        /// Datum waarop het besluit gepubliceerd wordt.
        /// </summary>
        [JsonPropertyName("publicatiedatum")]
        public DateOnly? Publicatiedatum { get; set; }

        /// <summary>
        /// Datum waarop het besluit verzonden is.
        /// </summary>
        [JsonPropertyName("verzenddatum")]
        public DateOnly? Verzenddatum { get; set; }

        /// <summary>
        /// De datum tot wanneer verweer tegen het besluit mogelijk is.
        /// </summary>
        [JsonPropertyName("uiterlijkeReactiedatum")]
        public DateOnly? UiterlijkeReactiedatum { get; set; }
    }

    public class OzBesluit
    {
        [JsonPropertyName("url")]
        public required Uri Url { get; set; }

        [JsonPropertyName("identificatie")]
        public required string Identificatie { get; set; }

        [JsonPropertyName("verantwoordelijkeOrganisatie")]
        public required string VerantwoordelijkeOrganisatie { get; set; }

        [JsonPropertyName("besluittype")]
        public required Uri Besluittype { get; set; }

        [JsonPropertyName("zaak")]
        public Uri? Zaak { get; set; }

        [JsonPropertyName("datum")]
        public required DateOnly Datum { get; set; }

        [JsonPropertyName("toelichting")]
        public string? Toelichting { get; set; }

        [JsonPropertyName("bestuursorgaan")]
        public string? Bestuursorgaan { get; set; }

        [JsonPropertyName("ingangsdatum")]
        public required DateOnly Ingangsdatum { get; set; }

        [JsonPropertyName("vervaldatum")]
        public DateOnly? Vervaldatum { get; set; }

        [JsonPropertyName("vervalreden")]
        public Vervalreden? Vervalreden { get; set; }

        [JsonPropertyName("publicatiedatum")]
        public DateOnly? Publicatiedatum { get; set; }

        [JsonPropertyName("verzenddatum")]
        public DateOnly? Verzenddatum { get; set; }

        [JsonPropertyName("uiterlijkeReactiedatum")]
        public DateOnly? UiterlijkeReactiedatum { get; set; }
    }

    /// <summary>
    /// De omschrijving die aangeeft op grond waarvan het besluit is of komt te vervallen.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumWithNullConverter<Vervalreden>))]
    public enum Vervalreden
    {
        /// <summary>
        /// Besluit met tijdelijke werking
        /// </summary>
        tijdelijk,

        /// <summary>
        /// Besluit ingetrokken door overheid
        /// </summary>
        ingetrokken_overheid,

        /// <summary>
        /// Besluit ingetrokken o.v.v. belanghebbende
        /// </summary>
        ingetrokken_belanghebbende
    }
}
