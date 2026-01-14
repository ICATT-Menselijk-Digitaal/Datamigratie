// using System.Text.Json.Serialization;

// namespace ZakenGenerator.Models;

// public record ZaaktypeCatalogus
// {
//     [JsonPropertyName("metadata")]
//     public CatalogusMetadata? Metadata { get; init; }

//     [JsonPropertyName("zaaktypen")]
//     public required IReadOnlyList<Zaaktype> Zaaktypen { get; init; }
// }

// public record CatalogusMetadata
// {
//     [JsonPropertyName("versie")]
//     public string? Versie { get; init; }

//     [JsonPropertyName("extractieDatum")]
//     public DateTime? ExtractieDatum { get; init; }

//     [JsonPropertyName("bronBestanden")]
//     public IReadOnlyList<string>? BronBestanden { get; init; }
// }

// public record Zaaktype
// {
//     [JsonPropertyName("identificatie")]
//     public string? Identificatie { get; init; }

//     [JsonPropertyName("omschrijving")]
//     public required string Omschrijving { get; init; }

//     [JsonPropertyName("omschrijvingGeneriek")]
//     public string? OmschrijvingGeneriek { get; init; }

//     [JsonPropertyName("toelichting")]
//     public string? Toelichting { get; init; }

//     [JsonPropertyName("doel")]
//     public string? Doel { get; init; }

//     [JsonPropertyName("aanleiding")]
//     public string? Aanleiding { get; init; }

//     [JsonPropertyName("bronBestand")]
//     public string? BronBestand { get; init; }

//     [JsonPropertyName("domein")]
//     public string? Domein { get; init; }

//     [JsonPropertyName("categorie")]
//     public ZaaktypeCategorie? Categorie { get; init; }

//     [JsonPropertyName("indicatieInternOfExtern")]
//     public string? IndicatieInternOfExtern { get; init; }

//     [JsonPropertyName("handelingInitiator")]
//     public string? HandelingInitiator { get; init; }

//     [JsonPropertyName("handelingBehandelaar")]
//     public string? HandelingBehandelaar { get; init; }

//     [JsonPropertyName("onderwerp")]
//     public string? Onderwerp { get; init; }

//     [JsonPropertyName("doorlooptijd")]
//     public string? Doorlooptijd { get; init; }

//     [JsonPropertyName("servicenorm")]
//     public string? Servicenorm { get; init; }

//     [JsonPropertyName("opschortingEnAanhoudingMogelijk")]
//     public bool OpschortingEnAanhoudingMogelijk { get; init; }

//     [JsonPropertyName("verlengingMogelijk")]
//     public bool VerlengingMogelijk { get; init; }

//     [JsonPropertyName("verlengingstermijn")]
//     public string? Verlengingstermijn { get; init; }

//     [JsonPropertyName("productenOfDiensten")]
//     public IReadOnlyList<string>? ProductenOfDiensten { get; init; }

//     [JsonPropertyName("beginGeldigheid")]
//     public string? BeginGeldigheid { get; init; }

//     [JsonPropertyName("eindeGeldigheid")]
//     public string? EindeGeldigheid { get; init; }

//     [JsonPropertyName("versiedatum")]
//     public string? Versiedatum { get; init; }

//     [JsonPropertyName("vertrouwelijkheidaanduiding")]
//     public string? Vertrouwelijkheidaanduiding { get; init; }

//     [JsonPropertyName("archivering")]
//     public ZaaktypeArchivering? Archivering { get; init; }

//     [JsonPropertyName("verantwoordelijke")]
//     public string? Verantwoordelijke { get; init; }

//     [JsonPropertyName("publicatieIndicatie")]
//     public bool PublicatieIndicatie { get; init; }

//     [JsonPropertyName("referentieproces")]
//     public Referentieproces? Referentieproces { get; init; }

//     [JsonPropertyName("catalogus")]
//     public string? Catalogus { get; init; }

//     [JsonPropertyName("statussen")]
//     public IReadOnlyList<ZaaktypeStatus>? Statussen { get; init; }

//     [JsonPropertyName("resultaten")]
//     public IReadOnlyList<ZaaktypeResultaat>? Resultaten { get; init; }

//     [JsonPropertyName("betrokkenetypen")]
//     public IReadOnlyList<Betrokkenetype>? Betrokkenetypen { get; init; }

//     [JsonPropertyName("documenttypen")]
//     public IReadOnlyList<ZaaktypeDocumenttype>? Documenttypen { get; init; }

//     [JsonPropertyName("besluittypen")]
//     public IReadOnlyList<ZaaktypeBesluittype>? Besluittypen { get; init; }

//     [JsonPropertyName("eigenschappen")]
//     public IReadOnlyList<ZaaktypeEigenschap>? Eigenschappen { get; init; }

//     [JsonPropertyName("gerelateerdeZaaktypen")]
//     public IReadOnlyList<GerelateerdeZaaktype>? GerelateerdeZaaktypen { get; init; }

//     [JsonPropertyName("wettelijkeGrondslag")]
//     public WettelijkeGrondslag? WettelijkeGrondslag { get; init; }
// }

// public record Referentieproces
// {
//     [JsonPropertyName("naam")]
//     public string? Naam { get; init; }

//     [JsonPropertyName("link")]
//     public string? Link { get; init; }
// }

// public record ZaaktypeArchivering
// {
//     [JsonPropertyName("bewaartermijn")]
//     public int? Bewaartermijn { get; init; }

//     [JsonPropertyName("bewaartermijnEenheid")]
//     public string? BewaartermijnEenheid { get; init; }

//     [JsonPropertyName("waardering")]
//     public string? Waardering { get; init; }

//     [JsonPropertyName("selectielijstItem")]
//     public string? SelectielijstItem { get; init; }

//     [JsonPropertyName("reviewPeriode")]
//     public int? ReviewPeriode { get; init; }
// }

// public record ZaaktypeStatus
// {
//     [JsonPropertyName("volgnummer")]
//     public required int Volgnummer { get; init; }

//     [JsonPropertyName("omschrijving")]
//     public required string Omschrijving { get; init; }

//     [JsonPropertyName("omschrijvingGeneriek")]
//     public string? OmschrijvingGeneriek { get; init; }

//     [JsonPropertyName("toelichting")]
//     public string? Toelichting { get; init; }

//     [JsonPropertyName("statustekst")]
//     public string? Statustekst { get; init; }

//     [JsonPropertyName("isStartstatus")]
//     public bool? IsStartstatus { get; init; }

//     [JsonPropertyName("isEindstatus")]
//     public bool? IsEindstatus { get; init; }

//     [JsonPropertyName("beginGeldigheid")]
//     public string? BeginGeldigheid { get; init; }

//     [JsonPropertyName("eindeGeldigheid")]
//     public string? EindeGeldigheid { get; init; }
// }

// public record ZaaktypeResultaat
// {
//     [JsonPropertyName("omschrijving")]
//     public required string Omschrijving { get; init; }

//     [JsonPropertyName("omschrijvingGeneriek")]
//     public string? OmschrijvingGeneriek { get; init; }

//     [JsonPropertyName("resultaattypeomschrijving")]
//     public string? Resultaattypeomschrijving { get; init; }

//     [JsonPropertyName("archivering")]
//     public ResultaatArchivering? Archivering { get; init; }

//     [JsonPropertyName("selectielijstItem")]
//     public string? SelectielijstItem { get; init; }

//     [JsonPropertyName("toelichting")]
//     public string? Toelichting { get; init; }
// }

// public record ResultaatArchivering
// {
//     [JsonPropertyName("waardering")]
//     public string? Waardering { get; init; }

//     [JsonPropertyName("bewaartermijn")]
//     public int? Bewaartermijn { get; init; }

//     [JsonPropertyName("bewaartermijnEenheid")]
//     public string? BewaartermijnEenheid { get; init; }
// }

// public record Betrokkenetype
// {
//     [JsonPropertyName("naam")]
//     public required string Naam { get; init; }

//     [JsonPropertyName("omschrijving")]
//     public string? Omschrijving { get; init; }

//     [JsonPropertyName("subjecttype")]
//     public string? Subjecttype { get; init; }
// }

// public record ZaaktypeDocumenttype
// {
//     [JsonPropertyName("volgnummer")]
//     public int? Volgnummer { get; init; }

//     [JsonPropertyName("naam")]
//     public string? Naam { get; init; }

//     [JsonPropertyName("omschrijving")]
//     public string? Omschrijving { get; init; }

//     [JsonPropertyName("documentcategorie")]
//     public string? Documentcategorie { get; init; }

//     [JsonPropertyName("richting")]
//     public string? Richting { get; init; }

//     [JsonPropertyName("vertrouwelijkheidaanduiding")]
//     public string? Vertrouwelijkheidaanduiding { get; init; }

//     [JsonPropertyName("verplicht")]
//     public bool? Verplicht { get; init; }
// }

// public record ZaaktypeBesluittype
// {
//     [JsonPropertyName("naam")]
//     public string? Naam { get; init; }

//     [JsonPropertyName("omschrijving")]
//     public string? Omschrijving { get; init; }

//     [JsonPropertyName("besluitcategorie")]
//     public string? Besluitcategorie { get; init; }

//     [JsonPropertyName("reactietermijnInDagen")]
//     public int? ReactietermijnInDagen { get; init; }

//     [JsonPropertyName("publicatieIndicatie")]
//     public bool PublicatieIndicatie { get; init; }

//     [JsonPropertyName("publicatietermijnInDagen")]
//     public int? PublicatietermijnInDagen { get; init; }
// }

// public record ZaaktypeEigenschap
// {
//     [JsonPropertyName("naam")]
//     public required string Naam { get; init; }

//     [JsonPropertyName("omschrijving")]
//     public string? Omschrijving { get; init; }

//     [JsonPropertyName("datatype")]
//     public string? Datatype { get; init; }

//     [JsonPropertyName("verplicht")]
//     public bool? Verplicht { get; init; }

//     [JsonPropertyName("opties")]
//     public IReadOnlyList<string>? Opties { get; init; }

//     [JsonPropertyName("waarde")]
//     public string? Waarde { get; init; }
// }

// public record ZaaktypeCategorie
// {
//     [JsonPropertyName("naam")]
//     public required string Naam { get; init; }

//     [JsonPropertyName("omschrijving")]
//     public string? Omschrijving { get; init; }

//     [JsonPropertyName("actief")]
//     public bool? Actief { get; init; }
// }

// public record GerelateerdeZaaktype
// {
//     [JsonPropertyName("zaaktypeIdentificatie")]
//     public required string ZaaktypeIdentificatie { get; init; }

//     [JsonPropertyName("aardRelatie")]
//     public string? AardRelatie { get; init; }
// }

// public record WettelijkeGrondslag
// {
//     [JsonPropertyName("wet")]
//     public string? Wet { get; init; }

//     [JsonPropertyName("artikel")]
//     public string? Artikel { get; init; }

//     [JsonPropertyName("omschrijving")]
//     public string? Omschrijving { get; init; }

//     [JsonPropertyName("url")]
//     public string? Url { get; init; }
// }
