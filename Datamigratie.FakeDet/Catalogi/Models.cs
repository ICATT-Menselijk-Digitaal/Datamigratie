using System.Text.Json.Serialization;

namespace Datamigratie.FakeDet.Catalogi.Omgevingswet;

/// <summary>
/// Root object voor de Zaaktypecatalogus Omgevingswet
/// </summary>
public class ZaaktypecatalogusRoot
{
    [JsonPropertyName("catalogus_metadata")]
    public required CatalogusMetadata CatalogusMetadata { get; init; }

    [JsonPropertyName("informatieobjecttypen")]
    public List<Informatieobjecttype>? Informatieobjecttypen { get; init; }

    [JsonPropertyName("besluittypen")]
    public List<Besluittype>? Besluittypen { get; init; }

    [JsonPropertyName("zaaktypen")]
    public required List<Zaaktype> Zaaktypen { get; init; }
}

/// <summary>
/// Metadata uit het hoofdcatalogusbestand (00)
/// </summary>
public class CatalogusMetadata
{
    /// <summary>
    /// Naam van de catalogus
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Versienummer van de catalogus
    /// </summary>
    [JsonPropertyName("versie")]
    public required string Versie { get; init; }

    /// <summary>
    /// RSIN identificatienummer
    /// </summary>
    [JsonPropertyName("rsin")]
    public required string Rsin { get; init; }

    /// <summary>
    /// Contactpersoon voor de catalogus
    /// </summary>
    [JsonPropertyName("contactpersoon")]
    public string? Contactpersoon { get; init; }

    /// <summary>
    /// Contact e-mailadres
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Begindatum geldigheid catalogus
    /// </summary>
    [JsonPropertyName("begindatum")]
    public DateTimeOffset? Begindatum { get; init; }

    /// <summary>
    /// Einddatum geldigheid catalogus
    /// </summary>
    [JsonPropertyName("einddatum")]
    public DateTimeOffset? Einddatum { get; init; }
}

/// <summary>
/// Informatieobjecttype uit de hoofdcatalogus
/// </summary>
public class Informatieobjecttype
{
    /// <summary>
    /// Unieke identificatie van het informatieobjecttype
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Omschrijving van het informatieobjecttype
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public required string Omschrijving { get; init; }

    /// <summary>
    /// Categorie-indeling
    /// </summary>
    [JsonPropertyName("categorie")]
    public string? Categorie { get; init; }

    /// <summary>
    /// Vertrouwelijkheidaanduiding
    /// </summary>
    [JsonPropertyName("vertrouwelijkheidaanduiding")]
    public string? Vertrouwelijkheidaanduiding { get; init; }

    /// <summary>
    /// Versiedatum
    /// </summary>
    [JsonPropertyName("versiedatum")]
    public DateTimeOffset? Versiedatum { get; init; }

    /// <summary>
    /// Begindatum geldigheid
    /// </summary>
    [JsonPropertyName("begindatum")]
    public DateTimeOffset? Begindatum { get; init; }

    /// <summary>
    /// Einddatum geldigheid
    /// </summary>
    [JsonPropertyName("einddatum")]
    public DateTimeOffset? Einddatum { get; init; }

    /// <summary>
    /// Gebruiksinstructie
    /// </summary>
    [JsonPropertyName("gebruiksinstructie")]
    public string? Gebruiksinstructie { get; init; }
}

/// <summary>
/// Besluittype uit de hoofdcatalogus
/// </summary>
public class Besluittype
{
    /// <summary>
    /// Unieke identificatie van het besluittype
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Omschrijving van het besluittype
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public required string Omschrijving { get; init; }

    /// <summary>
    /// Termijn voor reacties
    /// </summary>
    [JsonPropertyName("reactietermijn")]
    public string? Reactietermijn { get; init; }

    /// <summary>
    /// Of publicatie verplicht is
    /// </summary>
    [JsonPropertyName("publicatie_verplicht")]
    public bool? PublicatieVerplicht { get; init; }

    /// <summary>
    /// Tekst voor publicatie
    /// </summary>
    [JsonPropertyName("publicatietekst")]
    public string? Publicatietekst { get; init; }

    /// <summary>
    /// Termijn voor publicatie
    /// </summary>
    [JsonPropertyName("publicatietermijn")]
    public string? Publicatietermijn { get; init; }

    /// <summary>
    /// Versiedatum
    /// </summary>
    [JsonPropertyName("versiedatum")]
    public DateTimeOffset? Versiedatum { get; init; }

    /// <summary>
    /// Begindatum geldigheid
    /// </summary>
    [JsonPropertyName("begindatum")]
    public DateTimeOffset? Begindatum { get; init; }

    /// <summary>
    /// Einddatum geldigheid
    /// </summary>
    [JsonPropertyName("einddatum")]
    public DateTimeOffset? Einddatum { get; init; }
}

/// <summary>
/// Zaaktype met alle details
/// </summary>
public class Zaaktype
{
    /// <summary>
    /// Unieke identificatie van het zaaktype
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Omschrijving van het zaaktype
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public required string Omschrijving { get; init; }

    /// <summary>
    /// Versiedatum
    /// </summary>
    [JsonPropertyName("versiedatum")]
    public DateTimeOffset? Versiedatum { get; init; }

    /// <summary>
    /// Gedetailleerde informatie uit individueel zaaktypebestand
    /// </summary>
    [JsonPropertyName("details")]
    public required ZaaktypeDetails Details { get; init; }

    /// <summary>
    /// Statusworkflow
    /// </summary>
    [JsonPropertyName("statussen")]
    public List<Status>? Statussen { get; init; }

    /// <summary>
    /// Kwaliteitscontrole checklists per status
    /// </summary>
    [JsonPropertyName("checklists")]
    public List<Checklist>? Checklists { get; init; }

    /// <summary>
    /// Roltypen betrokken bij de zaak
    /// </summary>
    [JsonPropertyName("rollen")]
    public List<Rol>? Rollen { get; init; }

    /// <summary>
    /// Gerelateerde objecttypen
    /// </summary>
    [JsonPropertyName("objecten")]
    public List<ZaakObject>? Objecten { get; init; }

    /// <summary>
    /// Aangepaste eigenschappen
    /// </summary>
    [JsonPropertyName("eigenschappen")]
    public List<Eigenschap>? Eigenschappen { get; init; }

    /// <summary>
    /// Documentstroom informatie
    /// </summary>
    [JsonPropertyName("informatieobjecten")]
    public List<ZaakInformatieobject>? Informatieobjecten { get; init; }

    /// <summary>
    /// Van toepassing zijnde besluittypen
    /// </summary>
    [JsonPropertyName("besluiten")]
    public List<ZaakBesluit>? Besluiten { get; init; }

    /// <summary>
    /// Uitkomsttypen met archiveringsregels
    /// </summary>
    [JsonPropertyName("resultaten")]
    public List<Resultaat>? Resultaten { get; init; }

    /// <summary>
    /// Deelzaaktypen
    /// </summary>
    [JsonPropertyName("deelzaken")]
    public List<Deelzaak>? Deelzaken { get; init; }

    /// <summary>
    /// Gerelateerde zaaktypen
    /// </summary>
    [JsonPropertyName("gerelateerde_zaken")]
    public List<GerelateerdeZaak>? GerelateerdeZaken { get; init; }

    /// <summary>
    /// Procesdiagrammen (Afbeelding, Figuur)
    /// </summary>
    [JsonPropertyName("procesdiagrammen")]
    public List<Procesdiagram>? Procesdiagrammen { get; init; }
}

/// <summary>
/// Gedetailleerde informatie van een zaaktype
/// </summary>
public class ZaaktypeDetails
{
    /// <summary>
    /// Categorie-indeling
    /// </summary>
    [JsonPropertyName("categorie")]
    public string? Categorie { get; init; }

    /// <summary>
    /// Wettelijke grondslag voor het zaaktype
    /// </summary>
    [JsonPropertyName("wettelijke_grondslag")]
    public string? WettelijkeGrondslag { get; init; }

    /// <summary>
    /// Organisatie verantwoordelijk voor de afhandeling
    /// </summary>
    [JsonPropertyName("verantwoordelijke_organisatie")]
    public string? VerantwoordelijkeOrganisatie { get; init; }

    /// <summary>
    /// Servicenorm of afhandelingsniveau
    /// </summary>
    [JsonPropertyName("servicenorm")]
    public string? Servicenorm { get; init; }

    /// <summary>
    /// Doorlooptijd voor behandeling
    /// </summary>
    [JsonPropertyName("doorlooptijd")]
    public string? Doorlooptijd { get; init; }

    /// <summary>
    /// Toelichting op de doorlooptijd
    /// </summary>
    [JsonPropertyName("toelichting_doorlooptijd")]
    public string? ToelichtingDoorlooptijd { get; init; }

    /// <summary>
    /// Doel of oogmerk van het zaaktype
    /// </summary>
    [JsonPropertyName("doel")]
    public string? Doel { get; init; }

    /// <summary>
    /// Reikwijdte van het zaaktype
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; init; }

    /// <summary>
    /// Aanleiding of trigger wanneer dit zaaktype te gebruiken
    /// </summary>
    [JsonPropertyName("aanleiding")]
    public string? Aanleiding { get; init; }

    /// <summary>
    /// Openbare aanduiding tekst
    /// </summary>
    [JsonPropertyName("openbare_aanduiding")]
    public string? OpenbareAanduiding { get; init; }

    /// <summary>
    /// Publicatie indicatie
    /// </summary>
    [JsonPropertyName("publicatie_indicatie")]
    public string? PublicatieIndicatie { get; init; }

    /// <summary>
    /// Formaat voor referentienummers
    /// </summary>
    [JsonPropertyName("referentienummer_formaat")]
    public string? ReferentienummerFormaat { get; init; }

    /// <summary>
    /// Archiefregime classificatie
    /// </summary>
    [JsonPropertyName("archiefregime")]
    public string? Archiefregime { get; init; }

    /// <summary>
    /// Begindatum geldigheid
    /// </summary>
    [JsonPropertyName("begindatum")]
    public DateTimeOffset? Begindatum { get; init; }

    /// <summary>
    /// Einddatum geldigheid
    /// </summary>
    [JsonPropertyName("einddatum")]
    public DateTimeOffset? Einddatum { get; init; }
}

/// <summary>
/// Status in de workflow
/// </summary>
public class Status
{
    /// <summary>
    /// Volgnummer van de status
    /// </summary>
    [JsonPropertyName("volgnummer")]
    public required int Volgnummer { get; init; }

    /// <summary>
    /// Omschrijving van de status
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public required string Omschrijving { get; init; }

    /// <summary>
    /// Toelichting op de status
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }

    /// <summary>
    /// Type classificatie van de status
    /// </summary>
    [JsonPropertyName("statustype")]
    public string? Statustype { get; init; }

    /// <summary>
    /// Of dit een eindstatus is
    /// </summary>
    [JsonPropertyName("is_eindstatus")]
    public bool? IsEindstatus { get; init; }

    /// <summary>
    /// Doorlooptijd voor deze status
    /// </summary>
    [JsonPropertyName("doorlooptijd")]
    public string? Doorlooptijd { get; init; }

    /// <summary>
    /// Documenten vereist bij deze status
    /// </summary>
    [JsonPropertyName("vereiste_documenten")]
    public List<string>? VereissteDocumenten { get; init; }

    /// <summary>
    /// Rol verantwoordelijk voor deze status
    /// </summary>
    [JsonPropertyName("verantwoordelijke_rol")]
    public string? VerantwoordelijkeRol { get; init; }
}

/// <summary>
/// Kwaliteitscontrole checklist per status
/// </summary>
public class Checklist
{
    /// <summary>
    /// Status waar deze checklist op van toepassing is
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    /// <summary>
    /// Items om te controleren
    /// </summary>
    [JsonPropertyName("checklist_items")]
    public required List<ChecklistItem> ChecklistItems { get; init; }
}

/// <summary>
/// Item in een checklist
/// </summary>
public class ChecklistItem
{
    /// <summary>
    /// Volgorde van het checklistitem
    /// </summary>
    [JsonPropertyName("volgnummer")]
    public int? Volgnummer { get; init; }

    /// <summary>
    /// Checklistitem omschrijving
    /// </summary>
    [JsonPropertyName("item")]
    public string? Item { get; init; }

    /// <summary>
    /// Toelichting op het item
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }

    /// <summary>
    /// Of dit item verplicht is
    /// </summary>
    [JsonPropertyName("verplicht")]
    public bool? Verplicht { get; init; }
}

/// <summary>
/// Roltype betrokken bij de zaak
/// </summary>
public class Rol
{
    /// <summary>
    /// Roltype identificatie
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Omschrijving van de rol
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public required string Omschrijving { get; init; }

    /// <summary>
    /// Categorie van de rol (initiator, behandelaar, adviseur, etc.)
    /// </summary>
    [JsonPropertyName("categorie")]
    public string? Categorie { get; init; }

    /// <summary>
    /// Rechten toegekend aan deze rol
    /// </summary>
    [JsonPropertyName("rechten")]
    public List<string>? Rechten { get; init; }

    /// <summary>
    /// Toelichting op de rol
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }
}

/// <summary>
/// Gerelateerd objecttype
/// </summary>
public class ZaakObject
{
    /// <summary>
    /// Objecttype identificatie
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Omschrijving van het objecttype
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public required string Omschrijving { get; init; }

    /// <summary>
    /// Categorie (persoon, eigendom, adres, etc.)
    /// </summary>
    [JsonPropertyName("categorie")]
    public string? Categorie { get; init; }

    /// <summary>
    /// Of dit object verplicht is
    /// </summary>
    [JsonPropertyName("verplicht")]
    public bool? Verplicht { get; init; }

    /// <summary>
    /// Toelichting op het objecttype
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }
}

/// <summary>
/// Aangepaste eigenschap van een zaaktype
/// </summary>
public class Eigenschap
{
    /// <summary>
    /// Naam van de eigenschap
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Omschrijving van de eigenschap
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public required string Omschrijving { get; init; }

    /// <summary>
    /// Datatype van de eigenschap
    /// </summary>
    [JsonPropertyName("datatype")]
    public string? Datatype { get; init; }

    /// <summary>
    /// Formaatspecificatie
    /// </summary>
    [JsonPropertyName("formaat")]
    public string? Formaat { get; init; }

    /// <summary>
    /// Lijst van toegestane waarden voor enumeraties
    /// </summary>
    [JsonPropertyName("toegestane_waarden")]
    public List<string>? ToegestaneWaarden { get; init; }

    /// <summary>
    /// Of deze eigenschap verplicht is
    /// </summary>
    [JsonPropertyName("verplicht")]
    public bool? Verplicht { get; init; }

    /// <summary>
    /// Toelichting op de eigenschap
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }
}

/// <summary>
/// Documentstroom informatie
/// </summary>
public class ZaakInformatieobject
{
    /// <summary>
    /// Referentie naar informatieobjecttype uit catalogus
    /// </summary>
    [JsonPropertyName("informatieobjecttype_id")]
    public required string InformatieobjecttypeId { get; init; }

    /// <summary>
    /// Richting van het document
    /// </summary>
    [JsonPropertyName("richting")]
    public required DocumentRichting Richting { get; init; }

    /// <summary>
    /// Volgnummer in het proces
    /// </summary>
    [JsonPropertyName("volgnummer")]
    public int? Volgnummer { get; init; }

    /// <summary>
    /// Status wanneer dit document relevant is
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Of dit document verplicht is
    /// </summary>
    [JsonPropertyName("verplicht")]
    public bool? Verplicht { get; init; }

    /// <summary>
    /// Aanvullende omschrijving of context
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }
}

/// <summary>
/// Richting van een document
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentRichting
{
    [JsonPropertyName("inkomend")]
    Inkomend,

    [JsonPropertyName("uitgaand")]
    Uitgaand,

    [JsonPropertyName("intern")]
    Intern
}

/// <summary>
/// Besluit gekoppeld aan een zaaktype
/// </summary>
public class ZaakBesluit
{
    /// <summary>
    /// Referentie naar besluittype uit catalogus
    /// </summary>
    [JsonPropertyName("besluittype_id")]
    public required string BesluittypeId { get; init; }

    /// <summary>
    /// Status wanneer dit besluit van toepassing is
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Of dit besluit verplicht is
    /// </summary>
    [JsonPropertyName("verplicht")]
    public bool? Verplicht { get; init; }

    /// <summary>
    /// Toelichting wanneer/hoe dit besluit te gebruiken
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }
}

/// <summary>
/// Resultaat/uitkomsttype met archiveringsregels
/// </summary>
public class Resultaat
{
    /// <summary>
    /// Resultaattype identificatie
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Omschrijving van het resultaat
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public required string Omschrijving { get; init; }

    /// <summary>
    /// Toelichting op het resultaat
    /// </summary>
    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }

    /// <summary>
    /// Categorie-indeling
    /// </summary>
    [JsonPropertyName("categorie")]
    public string? Categorie { get; init; }

    /// <summary>
    /// Hoe lang documenten bewaard moeten worden
    /// </summary>
    [JsonPropertyName("bewaartermijn")]
    public string? Bewaartermijn { get; init; }

    /// <summary>
    /// Actie na bewaartermijn
    /// </summary>
    [JsonPropertyName("bewaringsactie")]
    public Bewaringsactie? Bewaringsactie { get; init; }

    /// <summary>
    /// Notities over archiveringshandelingen
    /// </summary>
    [JsonPropertyName("archiefnotities")]
    public string? Archiefnotities { get; init; }
}

/// <summary>
/// Actie na bewaartermijn
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Bewaringsactie
{
    [JsonPropertyName("vernietigen")]
    Vernietigen,

    [JsonPropertyName("bewaren")]
    Bewaren,

    [JsonPropertyName("overbrengen")]
    Overbrengen
}

/// <summary>
/// Deelzaaktype
/// </summary>
public class Deelzaak
{
    /// <summary>
    /// Referentie naar ander zaaktype gebruikt als deelzaak
    /// </summary>
    [JsonPropertyName("zaaktype_id")]
    public required string ZaaktypeId { get; init; }

    /// <summary>
    /// Omschrijving van de relatie
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Of deze deelzaak verplicht is
    /// </summary>
    [JsonPropertyName("verplicht")]
    public bool? Verplicht { get; init; }

    /// <summary>
    /// Hoeveel instanties toegestaan (bijv. '0..n', '1..1')
    /// </summary>
    [JsonPropertyName("multipliciteit")]
    public string? Multipliciteit { get; init; }
}

/// <summary>
/// Gerelateerd zaaktype
/// </summary>
public class GerelateerdeZaak
{
    /// <summary>
    /// Referentie naar ander gerelateerd zaaktype
    /// </summary>
    [JsonPropertyName("zaaktype_id")]
    public required string ZaaktypeId { get; init; }

    /// <summary>
    /// Type relatie (volgt op, gaat vooraf aan, gerelateerd aan, etc.)
    /// </summary>
    [JsonPropertyName("relatietype")]
    public required string Relatietype { get; init; }

    /// <summary>
    /// Omschrijving van de relatie
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Richting van de relatie
    /// </summary>
    [JsonPropertyName("richting")]
    public RelatieRichting? Richting { get; init; }
}

/// <summary>
/// Richting van een relatie
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RelatieRichting
{
    [JsonPropertyName("inkomend")]
    Inkomend,

    [JsonPropertyName("uitgaand")]
    Uitgaand,

    [JsonPropertyName("bidirectioneel")]
    Bidirectioneel
}

/// <summary>
/// Procesdiagram (Afbeelding, Figuur)
/// </summary>
public class Procesdiagram
{
    /// <summary>
    /// Type diagram (proces, figuur, etc.)
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Titel van het diagram
    /// </summary>
    [JsonPropertyName("titel")]
    public string? Titel { get; init; }

    /// <summary>
    /// Omschrijving van het diagram
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Referentie naar diagrambestand of embedded data
    /// </summary>
    [JsonPropertyName("bestandsreferentie")]
    public string? Bestandsreferentie { get; init; }
}
