using System.Text.Json.Serialization;

namespace Datamigratie.FakeDet.DataGeneration.Zaken.Models;

[JsonConverter(typeof(JsonStringEnumConverter<AdresType>))]
public enum AdresType
{
    [JsonStringEnumMemberName("correspondentie_adres")]
    CorrespondentieAdres,

    [JsonStringEnumMemberName("inschrijf_adres_persoon")]
    InschrijfAdresPersoon,

    [JsonStringEnumMemberName("verblijf_adres_persoon")]
    VerblijfAdresPersoon,

    [JsonStringEnumMemberName("post_adres_bedrijf")]
    PostAdresBedrijf,

    [JsonStringEnumMemberName("vestigings_adres_bedrijf")]
    VestigingsAdresBedrijf
}

[JsonConverter(typeof(JsonStringEnumConverter<Kanaal>))]
public enum Kanaal
{
    [JsonStringEnumMemberName("balie")]
    Balie,

    [JsonStringEnumMemberName("email")]
    Email,

    [JsonStringEnumMemberName("post")]
    Post,

    [JsonStringEnumMemberName("telefoon")]
    Telefoon,

    [JsonStringEnumMemberName("internet")]
    Internet,

    [JsonStringEnumMemberName("formulier")]
    Formulier
}

[JsonConverter(typeof(JsonStringEnumConverter<Geslacht>))]
public enum Geslacht
{
    [JsonStringEnumMemberName("man")]
    Man,

    [JsonStringEnumMemberName("vrouw")]
    Vrouw,

    [JsonStringEnumMemberName("onbekend")]
    Onbekend
}

[JsonConverter(typeof(JsonStringEnumConverter<BewaartermijnWaardering>))]
public enum BewaartermijnWaardering
{
    [JsonStringEnumMemberName("bewaren")]
    Bewaren,

    [JsonStringEnumMemberName("vernietigen")]
    Vernietigen
}

[JsonConverter(typeof(JsonStringEnumConverter<Betaalstatus>))]
public enum Betaalstatus
{
    [JsonStringEnumMemberName("geslaagd")]
    Geslaagd,

    [JsonStringEnumMemberName("niet_geslaagd")]
    NietGeslaagd,

    [JsonStringEnumMemberName("in_behandeling")]
    InBehandeling,

    [JsonStringEnumMemberName("geannuleerd")]
    Geannuleerd
}

[JsonConverter(typeof(JsonStringEnumConverter<Taaktype>))]
public enum Taaktype
{
    [JsonStringEnumMemberName("standaard")]
    Standaard,

    [JsonStringEnumMemberName("iburgerzaken")]
    Iburgerzaken,

    [JsonStringEnumMemberName("extern")]
    Extern,

    [JsonStringEnumMemberName("extern_ketenpartner")]
    ExternKetenpartner
}

[JsonConverter(typeof(JsonStringEnumConverter<TaakHistorieTypeWijziging>))]
public enum TaakHistorieTypeWijziging
{
    [JsonStringEnumMemberName("status")]
    Status,

    [JsonStringEnumMemberName("overdragen")]
    Overdragen,

    [JsonStringEnumMemberName("document")]
    Document,

    [JsonStringEnumMemberName("streefdatum")]
    Streefdatum,

    [JsonStringEnumMemberName("fataledatum")]
    Fataledatum,

    [JsonStringEnumMemberName("opschorttermijn")]
    Opschorttermijn
}

[JsonConverter(typeof(JsonStringEnumConverter<ZaakRelatietype>))]
public enum ZaakRelatietype
{
    [JsonStringEnumMemberName("hoofdzaak")]
    Hoofdzaak,

    [JsonStringEnumMemberName("deelzaak")]
    Deelzaak,

    [JsonStringEnumMemberName("gerelateerde_zaak")]
    GerelateerdeZaak,

    [JsonStringEnumMemberName("voorafgaande_zaak")]
    VoorafgaandeZaak,

    [JsonStringEnumMemberName("vervolgzaak")]
    Vervolgzaak
}

[JsonConverter(typeof(JsonStringEnumConverter<ZaakBetrokkenetype>))]
public enum ZaakBetrokkenetype
{
    [JsonStringEnumMemberName("belanghebbende")]
    Belanghebbende,

    [JsonStringEnumMemberName("gemachtigde")]
    Gemachtigde,

    [JsonStringEnumMemberName("medeaanvrager")]
    Medeaanvrager,

    [JsonStringEnumMemberName("melder")]
    Melder,

    [JsonStringEnumMemberName("plaatsvervanger")]
    Plaatsvervanger,

    [JsonStringEnumMemberName("overig")]
    Overig
}

[JsonConverter(typeof(JsonStringEnumConverter<ZaakHistorieTypeWijziging>))]
public enum ZaakHistorieTypeWijziging
{
    [JsonStringEnumMemberName("overdragen")]
    Overdragen,

    [JsonStringEnumMemberName("overbrengen")]
    Overbrengen,

    [JsonStringEnumMemberName("status")]
    Status,

    [JsonStringEnumMemberName("kanaal")]
    Kanaal,

    [JsonStringEnumMemberName("streefdatum")]
    Streefdatum,

    [JsonStringEnumMemberName("startdatum")]
    Startdatum,

    [JsonStringEnumMemberName("fataledatum")]
    Fataledatum,

    [JsonStringEnumMemberName("vertrouwelijkheid")]
    Vertrouwelijkheid,

    [JsonStringEnumMemberName("zaak_omschrijving")]
    ZaakOmschrijving,

    [JsonStringEnumMemberName("reden_starten_zaak")]
    RedenStartenZaak,

    [JsonStringEnumMemberName("verlenging_bewaartermijn")]
    VerlengingBewaartermijn,

    [JsonStringEnumMemberName("reviewtermijn")]
    Reviewtermijn,

    [JsonStringEnumMemberName("overdragen_ssa")]
    OverdragenSsa,

    [JsonStringEnumMemberName("overbrengen_extern")]
    OverbrengenExtern,

    [JsonStringEnumMemberName("heropenen_zaak")]
    HeropenenZaak,

    [JsonStringEnumMemberName("nieuw_document")]
    NieuwDocument,

    [JsonStringEnumMemberName("document_ontkoppeld")]
    DocumentOntkoppeld,

    [JsonStringEnumMemberName("document_verwijderd")]
    DocumentVerwijderd,

    [JsonStringEnumMemberName("document_unlocked")]
    DocumentUnlocked,

    [JsonStringEnumMemberName("aanvrager")]
    Aanvrager,

    [JsonStringEnumMemberName("betrokkenen")]
    Betrokkenen,

    [JsonStringEnumMemberName("zaakresultaat")]
    Zaakresultaat,

    [JsonStringEnumMemberName("ogone_post_sale")]
    OgonePostSale,

    [JsonStringEnumMemberName("ingangsdatum_besluit")]
    IngangsdatumBesluit,

    [JsonStringEnumMemberName("vervaldatum_besluit")]
    VervaldatumBesluit,

    [JsonStringEnumMemberName("bewaartermijn")]
    Bewaartermijn,

    [JsonStringEnumMemberName("opschorttermijn")]
    Opschorttermijn,

    [JsonStringEnumMemberName("autorisatie")]
    Autorisatie,

    [JsonStringEnumMemberName("organisatie")]
    Organisatie,

    [JsonStringEnumMemberName("proces_herstarten")]
    ProcesHerstarten,

    [JsonStringEnumMemberName("betaling_verwerkt")]
    BetalingVerwerkt,

    [JsonStringEnumMemberName("bag_koppeling")]
    BagKoppeling,

    [JsonStringEnumMemberName("zaak_koppeling")]
    ZaakKoppeling,

    [JsonStringEnumMemberName("contact_koppeling")]
    ContactKoppeling,

    [JsonStringEnumMemberName("externe_taak")]
    ExterneTaak,

    [JsonStringEnumMemberName("externe_taak_ketenpartner")]
    ExterneTaakKetenpartner,

    [JsonStringEnumMemberName("locatie")]
    Locatie,

    [JsonStringEnumMemberName("notitie")]
    Notitie,

    [JsonStringEnumMemberName("zaaktype")]
    Zaaktype,

    [JsonStringEnumMemberName("document_status")]
    DocumentStatus,

    [JsonStringEnumMemberName("metadataelement")]
    Metadataelement,

    [JsonStringEnumMemberName("documenttype")]
    Documenttype
}

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

[JsonConverter(typeof(JsonStringEnumConverter<DocumentRichting>))]
public enum DocumentRichting
{
    [JsonStringEnumMemberName("inkomend")]
    Inkomend,

    [JsonStringEnumMemberName("intern")]
    Intern,

    [JsonStringEnumMemberName("uitgaand")]
    Uitgaand
}

[JsonConverter(typeof(JsonStringEnumConverter<DocumentHistorieTypeWijziging>))]
public enum DocumentHistorieTypeWijziging
{
    [JsonStringEnumMemberName("status")]
    Status,

    [JsonStringEnumMemberName("versie")]
    Versie,

    [JsonStringEnumMemberName("metadata")]
    Metadata,

    [JsonStringEnumMemberName("publicatie")]
    Publicatie
}
