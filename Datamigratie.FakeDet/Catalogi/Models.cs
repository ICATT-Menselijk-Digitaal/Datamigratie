using System.Text.Json.Serialization;

namespace Datamigratie.FakeDet.Catalogi;

/// <summary>
/// Schema voor opslag van zaaktype-gegevens geëxtraheerd uit Omgevingswet Excel bestanden
/// </summary>
public sealed class ZaaktypeCatalogus
{
    /// <summary>
    /// Metadata over de extractie
    /// </summary>
    [JsonPropertyName("metadata")]
    public required CatalogusMetadata Metadata { get; init; }

    /// <summary>
    /// Lijst van zaaktypen
    /// </summary>
    [JsonPropertyName("zaaktypen")]
    public required List<Zaaktype> Zaaktypen { get; init; }
}

/// <summary>
/// Metadata over de extractie
/// </summary>
public sealed class CatalogusMetadata
{
    /// <summary>
    /// Versie van de catalogus
    /// </summary>
    [JsonPropertyName("versie")]
    public required string Versie { get; init; }

    /// <summary>
    /// Datum/tijd van extractie
    /// </summary>
    [JsonPropertyName("extractieDatum")]
    public required DateTime ExtractieDatum { get; init; }

    /// <summary>
    /// Lijst van bronbestanden
    /// </summary>
    [JsonPropertyName("bronBestanden")]
    public required List<string> BronBestanden { get; init; }
}

/// <summary>
/// Zaaktype definitie
/// </summary>
public sealed class Zaaktype
{
    /// <summary>
    /// Unieke functionele identificatie van het zaaktype
    /// </summary>
    [JsonPropertyName("functioneleIdentificatie")]
    public required string FunctioneleIdentificatie { get; init; }

    /// <summary>
    /// Naam van het zaaktype
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Uitgebreide omschrijving van het zaaktype
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Toelichting op het zaaktype
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }

    /// <summary>
    /// Naam van het bronbestand waaruit dit zaaktype is geëxtraheerd
    /// </summary>
    [JsonPropertyName("bronBestand")]
    public string? BronBestand { get; init; }

    /// <summary>
    /// Categorie van het zaaktype
    /// </summary>
    [JsonPropertyName("categorie")]
    public Referentie? Categorie { get; init; }

    /// <summary>
    /// Domein waartoe het zaaktype behoort (bijv. Omgevingswet)
    /// </summary>
    [JsonPropertyName("domein")]
    public string? Domein { get; init; }

    /// <summary>
    /// Is het zaaktype voor intern of extern gebruik
    /// </summary>
    [JsonPropertyName("internExtern")]
    public InternExtern? InternExtern { get; init; }

    /// <summary>
    /// Actie die uitgevoerd wordt om zaak te starten
    /// </summary>
    [JsonPropertyName("handelingInitiator")]
    public string? HandelingInitiator { get; init; }

    /// <summary>
    /// Onderwerp van het zaaktype
    /// </summary>
    [JsonPropertyName("onderwerp")]
    public string? Onderwerp { get; init; }

    /// <summary>
    /// Aanleiding voor het zaaktype
    /// </summary>
    [JsonPropertyName("aanleiding")]
    public string? Aanleiding { get; init; }

    /// <summary>
    /// Doorlooptijd configuratie
    /// </summary>
    [JsonPropertyName("doorlooptijd")]
    public Doorlooptijd? Doorlooptijd { get; init; }

    /// <summary>
    /// Geldigheidsperiode
    /// </summary>
    [JsonPropertyName("geldigheid")]
    public Geldigheid? Geldigheid { get; init; }

    /// <summary>
    /// Is het zaaktype vertrouwelijk
    /// </summary>
    [JsonPropertyName("vertrouwelijk")]
    public bool? Vertrouwelijk { get; init; }

    /// <summary>
    /// Archiveringsinstellingen op zaaktype niveau
    /// </summary>
    [JsonPropertyName("archivering")]
    public Archivering? Archivering { get; init; }

    /// <summary>
    /// Verantwoordelijke afdeling/groep
    /// </summary>
    [JsonPropertyName("verantwoordelijke")]
    public Verantwoordelijke? Verantwoordelijke { get; init; }

    /// <summary>
    /// Is verlenging van de doorlooptijd mogelijk
    /// </summary>
    [JsonPropertyName("verlengingMogelijk")]
    public bool? VerlengingMogelijk { get; init; }

    /// <summary>
    /// Mogelijke statussen van zaken van dit type
    /// </summary>
    [JsonPropertyName("statussen")]
    public List<Zaakstatus>? Statussen { get; init; }

    /// <summary>
    /// Mogelijke resultaten van zaken van dit type
    /// </summary>
    [JsonPropertyName("resultaten")]
    public List<Resultaat>? Resultaten { get; init; }

    /// <summary>
    /// Mogelijke besluittypen bij zaken van dit type
    /// </summary>
    [JsonPropertyName("besluittypen")]
    public List<Besluittype>? Besluittypen { get; init; }

    /// <summary>
    /// Toegestane documenttypen bij zaken van dit type
    /// </summary>
    [JsonPropertyName("documenttypen")]
    public List<Documenttype>? Documenttypen { get; init; }

    /// <summary>
    /// Mogelijke betrokkenen bij zaken van dit type
    /// </summary>
    [JsonPropertyName("betrokkenetypen")]
    public List<Betrokkenetype>? Betrokkenetypen { get; init; }

    /// <summary>
    /// Zaaktypen die gerelateerd kunnen worden
    /// </summary>
    [JsonPropertyName("gerelateerdeZaaktypen")]
    public List<GerelateerdeZaaktype>? GerelateerdeZaaktypen { get; init; }

    /// <summary>
    /// Zaaktype-specifieke eigenschappen/attributen
    /// </summary>
    [JsonPropertyName("eigenschappen")]
    public List<Eigenschap>? Eigenschappen { get; init; }

    /// <summary>
    /// Wettelijke grondslag
    /// </summary>
    [JsonPropertyName("wettelijkeGrondslag")]
    public WettelijkeGrondslag? WettelijkeGrondslag { get; init; }

    /// <summary>
    /// Producten en diensten gekoppeld aan dit zaaktype
    /// </summary>
    [JsonPropertyName("productenDiensten")]
    public List<ProductDienst>? ProductenDiensten { get; init; }
}

/// <summary>
/// Referentie met naam en optionele omschrijving
/// </summary>
public sealed class Referentie
{
    /// <summary>
    /// Naam
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Omschrijving
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Is actief
    /// </summary>
    [JsonPropertyName("actief")]
    public bool? Actief { get; init; }
}

/// <summary>
/// Doorlooptijd configuratie
/// </summary>
public sealed class Doorlooptijd
{
    /// <summary>
    /// Gewenste doorlooptijd
    /// </summary>
    [JsonPropertyName("gewenst")]
    public int? Gewenst { get; init; }

    /// <summary>
    /// Maximale/wettelijke doorlooptijd
    /// </summary>
    [JsonPropertyName("vereist")]
    public int? Vereist { get; init; }

    /// <summary>
    /// Eenheid van doorlooptijd
    /// </summary>
    [JsonPropertyName("eenheid")]
    public DoorlooptijdEenheid? Eenheid { get; init; }

    /// <summary>
    /// Mag de doorlooptijd worden aangepast
    /// </summary>
    [JsonPropertyName("aanpassenToegestaan")]
    public bool? AanpassenToegestaan { get; init; }
}

/// <summary>
/// Geldigheidsperiode
/// </summary>
public sealed class Geldigheid
{
    /// <summary>
    /// Datum begin geldigheid
    /// </summary>
    [JsonPropertyName("beginDatum")]
    public DateOnly? BeginDatum { get; init; }

    /// <summary>
    /// Datum einde geldigheid
    /// </summary>
    [JsonPropertyName("eindeDatum")]
    public DateOnly? EindeDatum { get; init; }
}

/// <summary>
/// Archiveringsinstellingen
/// </summary>
public sealed class Archivering
{
    /// <summary>
    /// Bewaartermijn
    /// </summary>
    [JsonPropertyName("bewaartermijn")]
    public int? Bewaartermijn { get; init; }

    /// <summary>
    /// Eenheid van bewaartermijn
    /// </summary>
    [JsonPropertyName("bewaartermijnEenheid")]
    public BewaartermijnEenheid? BewaartermijnEenheid { get; init; }

    /// <summary>
    /// Waardering voor archivering
    /// </summary>
    [JsonPropertyName("waardering")]
    public Waardering? Waardering { get; init; }

    /// <summary>
    /// Verwijzing naar selectielijst item
    /// </summary>
    [JsonPropertyName("selectielijstItem")]
    public string? SelectielijstItem { get; init; }

    /// <summary>
    /// Review periode in weken
    /// </summary>
    [JsonPropertyName("reviewPeriode")]
    public int? ReviewPeriode { get; init; }
}

/// <summary>
/// Verantwoordelijke afdeling/groep
/// </summary>
public sealed class Verantwoordelijke
{
    /// <summary>
    /// Verantwoordelijke afdeling
    /// </summary>
    [JsonPropertyName("afdeling")]
    public string? Afdeling { get; init; }

    /// <summary>
    /// Verantwoordelijke groep
    /// </summary>
    [JsonPropertyName("groep")]
    public string? Groep { get; init; }
}

/// <summary>
/// Status van een zaak
/// </summary>
public sealed class Zaakstatus
{
    /// <summary>
    /// Naam van de status
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Omschrijving van de status
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Volgorde van de status in het proces
    /// </summary>
    [JsonPropertyName("volgnummer")]
    public int? Volgnummer { get; init; }

    /// <summary>
    /// Is dit een eindstatus
    /// </summary>
    [JsonPropertyName("isEindstatus")]
    public bool? IsEindstatus { get; init; }

    /// <summary>
    /// Is dit een startstatus
    /// </summary>
    [JsonPropertyName("isStartstatus")]
    public bool? IsStartstatus { get; init; }

    /// <summary>
    /// Type status
    /// </summary>
    [JsonPropertyName("statusType")]
    public StatusType? StatusType { get; init; }
}

/// <summary>
/// Resultaat van een zaak
/// </summary>
public sealed class Resultaat
{
    /// <summary>
    /// Naam van het resultaat
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Omschrijving van het resultaat
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Categorie/type resultaat
    /// </summary>
    [JsonPropertyName("resultaatType")]
    public string? ResultaatType { get; init; }

    /// <summary>
    /// Archiveringsinstellingen voor dit resultaat
    /// </summary>
    [JsonPropertyName("archivering")]
    public Archivering? Archivering { get; init; }

    /// <summary>
    /// Referentie naar selectielijst
    /// </summary>
    [JsonPropertyName("selectielijstItem")]
    public string? SelectielijstItem { get; init; }
}

/// <summary>
/// Besluittype
/// </summary>
public sealed class Besluittype
{
    /// <summary>
    /// Naam van het besluittype
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Omschrijving van het besluittype
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Categorie van het besluit
    /// </summary>
    [JsonPropertyName("besluitcategorie")]
    public string? Besluitcategorie { get; init; }

    /// <summary>
    /// Termijn voor reactie in dagen
    /// </summary>
    [JsonPropertyName("reactietermijnInDagen")]
    public int? ReactietermijnInDagen { get; init; }

    /// <summary>
    /// Moet het besluit gepubliceerd worden
    /// </summary>
    [JsonPropertyName("publicatieIndicatie")]
    public bool? PublicatieIndicatie { get; init; }

    /// <summary>
    /// Termijn voor publicatie in dagen
    /// </summary>
    [JsonPropertyName("publicatietermijnInDagen")]
    public int? PublicatietermijnInDagen { get; init; }
}

/// <summary>
/// Documenttype
/// </summary>
public sealed class Documenttype
{
    /// <summary>
    /// Naam van het documenttype
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Omschrijving van het documenttype
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Categorie van het document
    /// </summary>
    [JsonPropertyName("documentcategorie")]
    public string? Documentcategorie { get; init; }

    /// <summary>
    /// Publicatieniveau van het document
    /// </summary>
    [JsonPropertyName("publicatieniveau")]
    public Publicatieniveau? Publicatieniveau { get; init; }

    /// <summary>
    /// Richting van het document
    /// </summary>
    [JsonPropertyName("richting")]
    public Richting? Richting { get; init; }

    /// <summary>
    /// Is dit documenttype verplicht
    /// </summary>
    [JsonPropertyName("verplicht")]
    public bool? Verplicht { get; init; }

    /// <summary>
    /// Volgnummer van het document
    /// </summary>
    [JsonPropertyName("volgnummer")]
    public int? Volgnummer { get; init; }
}

/// <summary>
/// Betrokkenetype (rol)
/// </summary>
public sealed class Betrokkenetype
{
    /// <summary>
    /// Naam van het betrokkenetype (bijv. aanvrager, gemachtigde)
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Omschrijving van de rol
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Type subject dat deze rol kan vervullen
    /// </summary>
    [JsonPropertyName("subjecttype")]
    public Subjecttype? Subjecttype { get; init; }
}

/// <summary>
/// Gerelateerd zaaktype
/// </summary>
public sealed class GerelateerdeZaaktype
{
    /// <summary>
    /// Functionele identificatie van gerelateerd zaaktype
    /// </summary>
    [JsonPropertyName("zaaktypeIdentificatie")]
    public string? ZaaktypeIdentificatie { get; init; }

    /// <summary>
    /// Type relatie
    /// </summary>
    [JsonPropertyName("aardRelatie")]
    public AardRelatie? AardRelatie { get; init; }

    /// <summary>
    /// Toelichting op de relatie
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }
}

/// <summary>
/// Eigenschap van een zaaktype
/// </summary>
public sealed class Eigenschap
{
    /// <summary>
    /// Naam van de eigenschap
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Datatype van de eigenschap
    /// </summary>
    [JsonPropertyName("datatype")]
    public required EigenschapDatatype Datatype { get; init; }

    /// <summary>
    /// Omschrijving van de eigenschap
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Is deze eigenschap verplicht
    /// </summary>
    [JsonPropertyName("verplicht")]
    public bool? Verplicht { get; init; }

    /// <summary>
    /// Mogelijke waarden bij datatype optie/opties
    /// </summary>
    [JsonPropertyName("opties")]
    public List<string>? Opties { get; init; }

    /// <summary>
    /// Default of vaste waarde
    /// </summary>
    [JsonPropertyName("waarde")]
    public string? Waarde { get; init; }

    /// <summary>
    /// Toelichting op de eigenschap
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }
}

/// <summary>
/// Wettelijke grondslag
/// </summary>
public sealed class WettelijkeGrondslag
{
    /// <summary>
    /// Naam van de wet
    /// </summary>
    [JsonPropertyName("wet")]
    public string? Wet { get; init; }

    /// <summary>
    /// Artikel nummer
    /// </summary>
    [JsonPropertyName("artikel")]
    public string? Artikel { get; init; }

    /// <summary>
    /// Omschrijving van de grondslag
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Link naar wettekst
    /// </summary>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }
}

/// <summary>
/// Product of dienst
/// </summary>
public sealed class ProductDienst
{
    /// <summary>
    /// Naam van het product of de dienst
    /// </summary>
    [JsonPropertyName("naam")]
    public string? Naam { get; init; }

    /// <summary>
    /// URL naar product/dienst informatie
    /// </summary>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }
}

#region Enums

/// <summary>
/// Intern of extern gebruik
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<InternExtern>))]
public enum InternExtern
{
    [JsonStringEnumMemberName("intern")]
    Intern,

    [JsonStringEnumMemberName("extern")]
    Extern,

    [JsonStringEnumMemberName("beide")]
    Beide
}

/// <summary>
/// Eenheid van doorlooptijd
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<DoorlooptijdEenheid>))]
public enum DoorlooptijdEenheid
{
    [JsonStringEnumMemberName("dagen")]
    Dagen,

    [JsonStringEnumMemberName("weken")]
    Weken,

    [JsonStringEnumMemberName("maanden")]
    Maanden
}

/// <summary>
/// Eenheid van bewaartermijn
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<BewaartermijnEenheid>))]
public enum BewaartermijnEenheid
{
    [JsonStringEnumMemberName("maanden")]
    Maanden,

    [JsonStringEnumMemberName("jaren")]
    Jaren
}

/// <summary>
/// Waardering voor archivering
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Waardering>))]
public enum Waardering
{
    [JsonStringEnumMemberName("bewaar")]
    Bewaar,

    [JsonStringEnumMemberName("vernietig")]
    Vernietig
}

/// <summary>
/// Type status
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<StatusType>))]
public enum StatusType
{
    [JsonStringEnumMemberName("nieuw")]
    Nieuw,

    [JsonStringEnumMemberName("in_behandeling")]
    InBehandeling,

    [JsonStringEnumMemberName("afgehandeld")]
    Afgehandeld,

    [JsonStringEnumMemberName("geannuleerd")]
    Geannuleerd
}

/// <summary>
/// Publicatieniveau van document
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Publicatieniveau>))]
public enum Publicatieniveau
{
    [JsonStringEnumMemberName("extern")]
    Extern,

    [JsonStringEnumMemberName("intern")]
    Intern,

    [JsonStringEnumMemberName("vertrouwelijk")]
    Vertrouwelijk
}

/// <summary>
/// Richting van document
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Richting>))]
public enum Richting
{
    [JsonStringEnumMemberName("inkomend")]
    Inkomend,

    [JsonStringEnumMemberName("intern")]
    Intern,

    [JsonStringEnumMemberName("uitgaand")]
    Uitgaand
}

/// <summary>
/// Type subject
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Subjecttype>))]
public enum Subjecttype
{
    [JsonStringEnumMemberName("persoon")]
    Persoon,

    [JsonStringEnumMemberName("bedrijf")]
    Bedrijf,

    [JsonStringEnumMemberName("beide")]
    Beide
}

/// <summary>
/// Aard van de relatie tussen zaaktypen
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<AardRelatie>))]
public enum AardRelatie
{
    [JsonStringEnumMemberName("onderwerp")]
    Onderwerp,

    [JsonStringEnumMemberName("vervolg")]
    Vervolg,

    [JsonStringEnumMemberName("bijdrage")]
    Bijdrage,

    [JsonStringEnumMemberName("gerelateerde_zaak")]
    GerelateerdeZaak,

    [JsonStringEnumMemberName("hoofdzaak")]
    Hoofdzaak,

    [JsonStringEnumMemberName("deelzaak")]
    Deelzaak
}

/// <summary>
/// Datatype van een eigenschap
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<EigenschapDatatype>))]
public enum EigenschapDatatype
{
    [JsonStringEnumMemberName("string")]
    String,

    [JsonStringEnumMemberName("boolean")]
    Boolean,

    [JsonStringEnumMemberName("datum")]
    Datum,

    [JsonStringEnumMemberName("datum_tijd")]
    DatumTijd,

    [JsonStringEnumMemberName("decimaal")]
    Decimaal,

    [JsonStringEnumMemberName("nummer")]
    Nummer,

    [JsonStringEnumMemberName("tekst")]
    Tekst,

    [JsonStringEnumMemberName("optie")]
    Optie,

    [JsonStringEnumMemberName("opties")]
    Opties
}

#endregion
