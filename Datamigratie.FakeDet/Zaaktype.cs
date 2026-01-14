using System.Text.Json.Serialization;

namespace Datamigratie.FakeDet;

#region Enums

/// <summary>
/// Is zaak voor intern of extern gebruik.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<InternExtern>))]
public enum InternExtern
{
    [JsonStringEnumMemberName("intern")]
    Intern,

    [JsonStringEnumMemberName("extern")]
    Extern
}

/// <summary>
/// Doelgroep van authenticatie.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<AuthenticatieDoelgroep>))]
public enum AuthenticatieDoelgroep
{
    [JsonStringEnumMemberName("burger")]
    Burger,

    [JsonStringEnumMemberName("bedrijf")]
    Bedrijf
}

/// <summary>
/// Niveau van authenticatie.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<AuthenticatieNiveau>))]
public enum AuthenticatieNiveau
{
    [JsonStringEnumMemberName("none")]
    None,

    [JsonStringEnumMemberName("digid_1")]
    Digid1,

    [JsonStringEnumMemberName("digid_2")]
    Digid2,

    [JsonStringEnumMemberName("digid_3")]
    Digid3,

    [JsonStringEnumMemberName("digid_4")]
    Digid4,

    [JsonStringEnumMemberName("eherkenning_1")]
    Eherkenning1,

    [JsonStringEnumMemberName("eherkenning_2")]
    Eherkenning2,

    [JsonStringEnumMemberName("eherkenning_3")]
    Eherkenning3,

    [JsonStringEnumMemberName("eherkenning_4")]
    Eherkenning4,

    [JsonStringEnumMemberName("eherkenning_5")]
    Eherkenning5,

    [JsonStringEnumMemberName("eherkenning_6")]
    Eherkenning6
}

/// <summary>
/// Bewaartermijn eenheid.
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
/// Waardering van de bewaartermijn.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<BewaartermijnWaardering>))]
public enum BewaartermijnWaardering
{
    [JsonStringEnumMemberName("bewaar")]
    Bewaar,

    [JsonStringEnumMemberName("vernietig")]
    Vernietig
}

/// <summary>
/// Default publicatieniveau.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<DocumentPublicatieniveau>))]
public enum DocumentPublicatieniveau
{
    [JsonStringEnumMemberName("extern")]
    Extern,

    [JsonStringEnumMemberName("intern")]
    Intern,

    [JsonStringEnumMemberName("vertrouwelijk")]
    Vertrouwelijk
}

#endregion

#region Base Classes

/// <summary>
/// Basis referentie met gemeenschappelijke eigenschappen.
/// </summary>
public class Referentie
{
    /// <summary>
    /// Naam.
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Omschrijving.
    /// </summary>
    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    /// <summary>
    /// Actief.
    /// </summary>
    [JsonPropertyName("actief")]
    public required bool Actief { get; init; }
}

#endregion

#region Reference Types

/// <summary>
/// Actie die uitgevoerd wordt om zaak of proces te starten.
/// </summary>
public class HandelingInitiator : Referentie;

/// <summary>
/// Categorie waarin zaak valt.
/// </summary>
public class Categorie : Referentie;

/// <summary>
/// Informatie voor Derden Categorie waarin zaak valt.
/// </summary>
public class Iv3Categorie : Referentie
{
    /// <summary>
    /// Externe code.
    /// </summary>
    [JsonPropertyName("externeCode")]
    public string? ExterneCode { get; init; }
}

/// <summary>
/// Status van een zaak.
/// </summary>
public class Zaakstatus : Referentie
{
    /// <summary>
    /// Code welke gebruikt wordt voor het uitwisselen van zaak informatie naar externe systemen zoals bijvoorbeeld via StUF-ZKN-DMS.
    /// </summary>
    [JsonPropertyName("uitwisselingscode")]
    public required string Uitwisselingscode { get; init; }

    /// <summary>
    /// Naam van status zoals deze getoond wordt in het publieke deel (burger en bedrijven loket) van de e-Suite.
    /// </summary>
    [JsonPropertyName("externeNaam")]
    public string? ExterneNaam { get; init; }

    /// <summary>
    /// Geeft aan of status gebruikt kan worden bij starten van zaak.
    /// </summary>
    [JsonPropertyName("start")]
    public required bool Start { get; init; }

    /// <summary>
    /// Geeft aan of status gebruikt kan worden bij beeindigen van zaak.
    /// </summary>
    [JsonPropertyName("eind")]
    public required bool Eind { get; init; }
}

/// <summary>
/// Resultaat van een zaak.
/// </summary>
public class Resultaat : Referentie
{
    /// <summary>
    /// Code welke gebruikt wordt voor het uitwisselen van zaak informatie naar externe systemen zoals bijvoorbeeld via StUF-ZKN-DMS.
    /// </summary>
    [JsonPropertyName("uitwisselingscode")]
    public required string Uitwisselingscode { get; init; }
}

/// <summary>
/// Selectielijst item.
/// </summary>
public class Selectielijstitem : Referentie
{
    /// <summary>
    /// Versie (jaar) van selectielijst item.
    /// </summary>
    [JsonPropertyName("jaar")]
    public int? Jaar { get; init; }

    /// <summary>
    /// Naam van domein.
    /// </summary>
    [JsonPropertyName("domein")]
    public string? Domein { get; init; }

    /// <summary>
    /// Naam van subdomein.
    /// </summary>
    [JsonPropertyName("subdomein")]
    public string? Subdomein { get; init; }

    /// <summary>
    /// Waardering van de bewaartermijn.
    /// </summary>
    [JsonPropertyName("bewaartermijnWaardering")]
    public BewaartermijnWaardering? BewaartermijnWaardering { get; init; }

    /// <summary>
    /// Bewaartermijn. Voor eenheid zie bewaartermijnEenheid.
    /// </summary>
    [JsonPropertyName("bewaartermijn")]
    public int? Bewaartermijn { get; init; }

    /// <summary>
    /// Bewaartermijn eenheid.
    /// </summary>
    [JsonPropertyName("bewaartermijnEenheid")]
    public BewaartermijnEenheid? BewaartermijnEenheid { get; init; }
}

/// <summary>
/// Type document.
/// </summary>
public class Documenttype : Referentie
{
    /// <summary>
    /// Aanduiding documentcategorie.
    /// </summary>
    [JsonPropertyName("documentcategorie")]
    public string? Documentcategorie { get; init; }

    /// <summary>
    /// Default publicatieniveau.
    /// </summary>
    [JsonPropertyName("publicatieniveau")]
    public required DocumentPublicatieniveau Publicatieniveau { get; init; }
}

/// <summary>
/// Document tag.
/// </summary>
public class DocumentTag : Referentie;

/// <summary>
/// Document titel.
/// </summary>
public class DocumentTitel : Referentie;

/// <summary>
/// Besluit categorie.
/// </summary>
public class Besluitcategorie : Referentie;

/// <summary>
/// Overzicht van een zaaktype.
/// </summary>
public class ZaaktypeOverzicht : Referentie
{
    /// <summary>
    /// Functionele identificatie.
    /// </summary>
    [JsonPropertyName("functioneleIdentificatie")]
    public required string FunctioneleIdentificatie { get; init; }
}

/// <summary>
/// Taak document.
/// </summary>
public class TaakDocument : Referentie
{
    /// <summary>
    /// Document naam.
    /// </summary>
    [JsonPropertyName("documentNaam")]
    public string? DocumentNaam { get; init; }

    /// <summary>
    /// Document template.
    /// </summary>
    [JsonPropertyName("documentTemplate")]
    public string? DocumentTemplate { get; init; }

    /// <summary>
    /// Template groep.
    /// </summary>
    [JsonPropertyName("templateGroep")]
    public string? TemplateGroep { get; init; }

    /// <summary>
    /// Type document.
    /// </summary>
    [JsonPropertyName("documenttype")]
    public Documenttype? Documenttype { get; init; }
}

#endregion

#region Complex Types

/// <summary>
/// Type besluit.
/// </summary>
public class Besluittype : Referentie
{
    /// <summary>
    /// Besluit categorie.
    /// </summary>
    [JsonPropertyName("besluitcategorie")]
    public required Besluitcategorie Besluitcategorie { get; init; }

    /// <summary>
    /// Reactietermijn in dagen.
    /// </summary>
    [JsonPropertyName("reactietermijnInDagen")]
    public required int ReactietermijnInDagen { get; init; }

    /// <summary>
    /// Publicatie indicatie.
    /// </summary>
    [JsonPropertyName("publicatieIndicatie")]
    public required bool PublicatieIndicatie { get; init; }

    /// <summary>
    /// Publicatietekst.
    /// </summary>
    [JsonPropertyName("publicatietekst")]
    public string? Publicatietekst { get; init; }

    /// <summary>
    /// Publicatie termijn in dagen.
    /// </summary>
    [JsonPropertyName("publicatietermijnInDagen")]
    public int? PublicatietermijnInDagen { get; init; }
}

/// <summary>
/// Authenticatie configuratie voor een zaaktype.
/// </summary>
public class ZaaktypeAuthenticatie
{
    /// <summary>
    /// Doelgroep van authenticatie.
    /// </summary>
    [JsonPropertyName("doelgroep")]
    public required AuthenticatieDoelgroep Doelgroep { get; init; }

    /// <summary>
    /// Niveau van authenticatie.
    /// </summary>
    [JsonPropertyName("niveau")]
    public required AuthenticatieNiveau Niveau { get; init; }
}

/// <summary>
/// Besluittype gekoppeld aan een zaaktype.
/// </summary>
public class ZaaktypeBesluittype
{
    /// <summary>
    /// Type besluit.
    /// </summary>
    [JsonPropertyName("besluittype")]
    public required Besluittype Besluittype { get; init; }

    /// <summary>
    /// Documenttype van document met besluit.
    /// </summary>
    [JsonPropertyName("documenttype")]
    public Documenttype? Documenttype { get; init; }

    /// <summary>
    /// Procestermijn in maanden.
    /// </summary>
    [JsonPropertyName("procestermijn")]
    public int? Procestermijn { get; init; }
}

/// <summary>
/// Documenttype gekoppeld aan een zaaktype.
/// </summary>
public class ZaaktypeDocumenttype
{
    /// <summary>
    /// Type document.
    /// </summary>
    [JsonPropertyName("documenttype")]
    public required Documenttype Documenttype { get; init; }

    /// <summary>
    /// DSP code.
    /// </summary>
    [JsonPropertyName("dspcode")]
    public string? Dspcode { get; init; }

    /// <summary>
    /// Mogelijke voorgedefinieerde document titels.
    /// </summary>
    [JsonPropertyName("titels")]
    public IReadOnlyList<DocumentTitel>? Titels { get; init; }
}

/// <summary>
/// Resultaat gekoppeld aan een zaaktype.
/// </summary>
public class ZaaktypeResultaat
{
    /// <summary>
    /// Resultaat van zaak.
    /// </summary>
    [JsonPropertyName("resultaat")]
    public required Resultaat Resultaat { get; init; }

    /// <summary>
    /// Selectielijst item.
    /// </summary>
    [JsonPropertyName("selectielijstitem")]
    public required Selectielijstitem Selectielijstitem { get; init; }

    /// <summary>
    /// Waardering van de bewaartermijn.
    /// </summary>
    [JsonPropertyName("bewaartermijnWaardering")]
    public BewaartermijnWaardering? BewaartermijnWaardering { get; init; }

    /// <summary>
    /// Bewaartermijn. Voor eenheid zie bewaartermijnEenheid.
    /// </summary>
    [JsonPropertyName("bewaartermijn")]
    public int? Bewaartermijn { get; init; }

    /// <summary>
    /// Bewaartermijn eenheid.
    /// </summary>
    [JsonPropertyName("bewaartermijnEenheid")]
    public BewaartermijnEenheid? BewaartermijnEenheid { get; init; }
}

/// <summary>
/// Taakdocument groep.
/// </summary>
public class TaakDocumentGroep
{
    /// <summary>
    /// Naam.
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Taak documenten.
    /// </summary>
    [JsonPropertyName("taakDocumenten")]
    public IReadOnlyList<TaakDocument>? TaakDocumenten { get; init; }
}

/// <summary>
/// Zaak start parameter.
/// </summary>
public class ZaakStartParameter
{
    /// <summary>
    /// Naam.
    /// </summary>
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    /// <summary>
    /// Type.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }
}

#endregion

#region Main Model

/// <summary>
/// Zaaktype definitie volgens GEMMA standaarden.
/// </summary>
public class Zaaktype : ZaaktypeOverzicht
{
    /// <summary>
    /// Actie die uitgevoerd wordt om zaak of proces te starten.
    /// </summary>
    [JsonPropertyName("handelingInitiator")]
    public required HandelingInitiator HandelingInitiator { get; init; }

    /// <summary>
    /// Is zaak voor intern of extern gebruik.
    /// </summary>
    [JsonPropertyName("internExtern")]
    public required InternExtern InternExtern { get; init; }

    /// <summary>
    /// Categorie waarin zaak valt.
    /// </summary>
    [JsonPropertyName("categorie")]
    public required Categorie Categorie { get; init; }

    /// <summary>
    /// Informatie voor Derden Categorie waarin zaak valt.
    /// </summary>
    [JsonPropertyName("iv3categorie")]
    public required Iv3Categorie Iv3Categorie { get; init; }

    /// <summary>
    /// Naam van afdeling waaraan nieuwe zaak wordt toegekend.
    /// </summary>
    [JsonPropertyName("afdeling")]
    public string? Afdeling { get; init; }

    /// <summary>
    /// Naam van groep waaraan nieuwe zaak wordt toegekend.
    /// </summary>
    [JsonPropertyName("groep")]
    public string? Groep { get; init; }

    /// <summary>
    /// Is zaak een intake.
    /// </summary>
    [JsonPropertyName("intake")]
    public required bool Intake { get; init; }

    /// <summary>
    /// Datum begin geldigheid.
    /// </summary>
    [JsonPropertyName("beginGeldigheidDatum")]
    public required DateOnly BeginGeldigheidDatum { get; init; }

    /// <summary>
    /// Datum einde geldigheid.
    /// </summary>
    [JsonPropertyName("eindeGeldigheidDatum")]
    public DateOnly? EindeGeldigheidDatum { get; init; }

    /// <summary>
    /// Gewenste doorlooptijd in dagen.
    /// </summary>
    [JsonPropertyName("doorlooptijdGewenst")]
    public int? DoorlooptijdGewenst { get; init; }

    /// <summary>
    /// Maximale doorlooptijd in dagen.
    /// </summary>
    [JsonPropertyName("doorlooptijdVereist")]
    public int? DoorlooptijdVereist { get; init; }

    /// <summary>
    /// Is aanpassen van doorlooptijd van zaak toegestaan.
    /// </summary>
    [JsonPropertyName("doorlooptijdAanpassenToegestaan")]
    public required bool DoorlooptijdAanpassenToegestaan { get; init; }

    /// <summary>
    /// Aantal dagen voorafgaand aan de streefdatum waarop de 1ste signalering plaats vindt.
    /// </summary>
    [JsonPropertyName("aantalDagenVoorStreefdatumVoorEersteSignalering")]
    public required int AantalDagenVoorStreefdatumVoorEersteSignalering { get; init; }

    /// <summary>
    /// Aantal dagen voorafgaand aan de streefdatum waarop de 2de signalering plaats vindt.
    /// </summary>
    [JsonPropertyName("aantalDagenVoorStreefdatumVoorTweedeSignalering")]
    public required int AantalDagenVoorStreefdatumVoorTweedeSignalering { get; init; }

    /// <summary>
    /// Aantal dagen voorafgaand aan de fataledatum waarop de 1ste signalering plaats vindt.
    /// </summary>
    [JsonPropertyName("aantalDagenVoorFataledatumVoorEersteSignalering")]
    public required int AantalDagenVoorFataledatumVoorEersteSignalering { get; init; }

    /// <summary>
    /// Aantal dagen voorafgaand aan de fataledatum waarop de 2de signalering plaats vindt.
    /// </summary>
    [JsonPropertyName("aantalDagenVoorFataledatumVoorTweedeSignalering")]
    public required int AantalDagenVoorFataledatumVoorTweedeSignalering { get; init; }

    /// <summary>
    /// Initiele status van nieuwe zaak.
    /// </summary>
    [JsonPropertyName("status")]
    public Zaakstatus? Status { get; init; }

    /// <summary>
    /// Archivering reviewperiode in weken.
    /// </summary>
    [JsonPropertyName("archiveringReviewPeriode")]
    public int? ArchiveringReviewPeriode { get; init; }

    /// <summary>
    /// Moet proces gestart worden bij aanmaken van nieuwe zaak.
    /// </summary>
    [JsonPropertyName("startenProces")]
    public required bool StartenProces { get; init; }

    /// <summary>
    /// Functionele identificatie van proces dat wordt gestart.
    /// </summary>
    [JsonPropertyName("proces")]
    public string? Proces { get; init; }

    /// <summary>
    /// Functionele identificatie van zaak startformulier.
    /// </summary>
    [JsonPropertyName("startformulier")]
    public string? Startformulier { get; init; }

    /// <summary>
    /// Versie nummer van zaak startformulier.
    /// </summary>
    [JsonPropertyName("startformulierVersie")]
    public long? StartformulierVersie { get; init; }

    /// <summary>
    /// Is zaak vertrouwelijk.
    /// </summary>
    [JsonPropertyName("vertrouwelijk")]
    public required bool Vertrouwelijk { get; init; }

    /// <summary>
    /// Mogelijke authenticatie methoden bij aanmaken nieuwe zaak.
    /// </summary>
    [JsonPropertyName("authenticaties")]
    public IReadOnlyList<ZaaktypeAuthenticatie>? Authenticaties { get; init; }

    /// <summary>
    /// Betreft dit een zaak met specifieke medewerker autorisatie.
    /// </summary>
    [JsonPropertyName("geautoriseerdVoorMedewerkers")]
    public required bool GeautoriseerdVoorMedewerkers { get; init; }

    /// <summary>
    /// Gebruikersnamen van medewerkers welke zijn geautoriseerd voor zaak.
    /// </summary>
    [JsonPropertyName("geautoriseerdeMedewerkers")]
    public IReadOnlyCollection<string>? GeautoriseerdeMedewerkers { get; init; }

    /// <summary>
    /// Moet er notificaties worden verstuurd voor de zaak.
    /// </summary>
    [JsonPropertyName("notificatiesVersturen")]
    public required bool NotificatiesVersturen { get; init; }

    /// <summary>
    /// Mogelijke statussen van zaak.
    /// </summary>
    [JsonPropertyName("statussen")]
    public IReadOnlyList<Zaakstatus>? Statussen { get; init; }

    /// <summary>
    /// Mogelijke resultaten van zaak.
    /// </summary>
    [JsonPropertyName("resultaten")]
    public IReadOnlyList<ZaaktypeResultaat>? Resultaten { get; init; }

    /// <summary>
    /// Mogelijke besluiten bij zaak.
    /// </summary>
    [JsonPropertyName("besluiten")]
    public IReadOnlyList<ZaaktypeBesluittype>? Besluiten { get; init; }

    /// <summary>
    /// Mogelijke documenttypen van documenten bij zaak.
    /// </summary>
    [JsonPropertyName("documenttypen")]
    public IReadOnlyList<ZaaktypeDocumenttype>? Documenttypen { get; init; }

    /// <summary>
    /// Tags welke gezet kunnen worden op documenten bij zaak.
    /// </summary>
    [JsonPropertyName("documentTags")]
    public IReadOnlyList<DocumentTag>? DocumentTags { get; init; }

    /// <summary>
    /// Zaaktypen waarvan zaken gekoppeld kunnen worden aan zaak.
    /// </summary>
    [JsonPropertyName("gekoppeldeZaaktypen")]
    public IReadOnlyList<ZaaktypeOverzicht>? GekoppeldeZaaktypen { get; init; }

    /// <summary>
    /// Taakdocument groepen.
    /// </summary>
    [JsonPropertyName("taakDocumentGroepen")]
    public IReadOnlyList<TaakDocumentGroep>? TaakDocumentGroepen { get; init; }

    /// <summary>
    /// Naam voor het samenvatting document.
    /// </summary>
    [JsonPropertyName("samenvattingDocumentNaam")]
    public required string SamenvattingDocumentNaam { get; init; }

    /// <summary>
    /// Zaak start parameters.
    /// </summary>
    [JsonPropertyName("zaakStartParameters")]
    public IReadOnlyList<ZaakStartParameter>? ZaakStartParameters { get; init; }

    /// <summary>
    /// Productaanvraagtype.
    /// </summary>
    [JsonPropertyName("productaanvraagtype")]
    public string? Productaanvraagtype { get; init; }
}

#endregion
