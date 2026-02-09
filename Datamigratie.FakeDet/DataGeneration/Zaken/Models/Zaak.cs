using System.Text.Json.Serialization;

namespace Datamigratie.FakeDet.DataGeneration.Zaken.Models;

public record Zaak
{
    [JsonPropertyName("functioneleIdentificatie")]
    public required string FunctioneleIdentificatie { get; init; }

    [JsonPropertyName("externeIdentificatie")]
    public required string ExterneIdentificatie { get; init; }

    [JsonPropertyName("omschrijving")]
    public required string Omschrijving { get; init; }

    [JsonPropertyName("redenStart")]
    public string? RedenStart { get; init; }

    [JsonPropertyName("zaaktype")]
    public required ZaaktypeOverzicht Zaaktype { get; init; }

    [JsonPropertyName("vertrouwelijk")]
    public required bool Vertrouwelijk { get; init; }

    [JsonPropertyName("behandelaar")]
    public string? Behandelaar { get; init; }

    [JsonPropertyName("initiator")]
    public Subject? Initiator { get; init; }

    [JsonPropertyName("afdeling")]
    public string? Afdeling { get; init; }

    [JsonPropertyName("groep")]
    public string? Groep { get; init; }

    [JsonPropertyName("aangemaaktDoor")]
    public required string AangemaaktDoor { get; init; }

    [JsonPropertyName("kanaal")]
    public required ZaakKanaal Kanaal { get; init; }

    [JsonPropertyName("creatieDatumTijd")]
    public required DateTime CreatieDatumTijd { get; init; }

    [JsonPropertyName("wijzigDatumTijd")]
    public DateTime? WijzigDatumTijd { get; init; }

    [JsonPropertyName("startdatum")]
    public DateOnly? Startdatum { get; init; }

    [JsonPropertyName("streefdatum")]
    public required DateOnly Streefdatum { get; init; }

    [JsonPropertyName("fataledatum")]
    public DateOnly? Fataledatum { get; init; }

    [JsonPropertyName("einddatum")]
    public DateOnly? Einddatum { get; init; }

    [JsonPropertyName("zaakStatus")]
    public required Zaakstatus ZaakStatus { get; init; }

    [JsonPropertyName("resultaat")]
    public ZaakResultaat? Resultaat { get; init; }

    [JsonPropertyName("intake")]
    public required bool Intake { get; init; }

    [JsonPropertyName("archiveerGegevens")]
    public ArchiveerGegevens? ArchiveerGegevens { get; init; }

    [JsonPropertyName("geolocatie")]
    public Geolocatie? Geolocatie { get; init; }

    [JsonPropertyName("historie")]
    public required IReadOnlyList<ZaakHistorie> Historie { get; init; }

    [JsonPropertyName("zaakdata")]
    public IReadOnlyList<ZaakDataElement>? Zaakdata { get; init; }

    [JsonPropertyName("organisatie")]
    public string? Organisatie { get; init; }

    [JsonPropertyName("geautoriseerdVoorMedewerkers")]
    public required bool GeautoriseerdVoorMedewerkers { get; init; }

    [JsonPropertyName("geautoriseerdeMedewerkers")]
    public IReadOnlyList<string>? GeautoriseerdeMedewerkers { get; init; }

    [JsonPropertyName("notities")]
    public IReadOnlyList<ZaakNotitie>? Notities { get; init; }

    [JsonPropertyName("procesGestart")]
    public required bool ProcesGestart { get; init; }

    [JsonPropertyName("heropend")]
    public required bool Heropend { get; init; }

    [JsonPropertyName("open")]
    public required bool Open { get; init; }

    [JsonPropertyName("vernietiging")]
    public required bool Vernietiging { get; init; }

    [JsonPropertyName("notificeerbaar")]
    public required bool Notificeerbaar { get; init; }

    [JsonPropertyName("gemigreerd")]
    public required bool Gemigreerd { get; init; }

    [JsonPropertyName("betrokkenen")]
    public IReadOnlyList<ZaakBetrokkene>? Betrokkenen { get; init; }

    [JsonPropertyName("documenten")]
    public IReadOnlyList<ZaakDocument>? Documenten { get; init; }

    [JsonPropertyName("betaalgegevens")]
    public Betaalgegevens? Betaalgegevens { get; init; }

    [JsonPropertyName("opschorttermijnStartdatum")]
    public DateOnly? OpschorttermijnStartdatum { get; init; }

    [JsonPropertyName("opschorttermijnEinddatum")]
    public DateOnly? OpschorttermijnEinddatum { get; init; }

    [JsonPropertyName("ztc1MigratiedatumTijd")]
    public DateTime? Ztc1MigratiedatumTijd { get; init; }

    [JsonPropertyName("taken")]
    public IReadOnlyList<Taak>? Taken { get; init; }

    [JsonPropertyName("bagObjecten")]
    public IReadOnlyList<BAGObject>? BagObjecten { get; init; }

    [JsonPropertyName("gekoppeldeZaken")]
    public IReadOnlyList<ZaakZaakKoppeling>? GekoppeldeZaken { get; init; }

    [JsonPropertyName("besluiten")]
    public IReadOnlyList<Besluit>? Besluiten { get; init; }

    [JsonPropertyName("contacten")]
    public IReadOnlyList<string>? Contacten { get; init; }
}

public record ZaaktypeOverzicht
{
    [JsonPropertyName("functioneleIdentificatie")]
    public string? FunctioneleIdentificatie { get; init; }

    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    [JsonPropertyName("domein")]
    public string? Domein { get; init; }
}

public record Zaakstatus
{
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    [JsonPropertyName("actief")]
    public required bool Actief { get; init; }

    [JsonPropertyName("uitwisselingscode")]
    public required string Uitwisselingscode { get; init; }

    [JsonPropertyName("externeNaam")]
    public string? ExterneNaam { get; init; }

    [JsonPropertyName("start")]
    public required bool Start { get; init; }

    [JsonPropertyName("eind")]
    public required bool Eind { get; init; }
}

public record ZaakResultaat
{
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    [JsonPropertyName("uitwisselingscode")]
    public required string Uitwisselingscode { get; init; }

    public required bool Actief { get; init; }
}

public record ArchiveerGegevens
{
    [JsonPropertyName("bewaartermijnEinddatum")]
    public DateOnly? BewaartermijnEinddatum { get; init; }

    [JsonPropertyName("bewaartermijnWaardering")]
    public BewaartermijnWaardering? BewaartermijnWaardering { get; init; }

    [JsonPropertyName("beperkingOpenbaarheid")]
    public bool? BeperkingOpenbaarheid { get; init; }

    [JsonPropertyName("beperkingOpenbaarheidReden")]
    public string? BeperkingOpenbaarheidReden { get; init; }
}

public record Geolocatie
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("coordinates")]
    public required double[] Coordinates { get; init; }
}

public record ZaakHistorie
{
    [JsonPropertyName("typeWijziging")]
    public required ZaakHistorieTypeWijziging TypeWijziging { get; init; }

    [JsonPropertyName("wijzigingDatum")]
    public DateOnly? WijzigingDatum { get; init; }

    [JsonPropertyName("gewijzigdDoor")]
    public string? GewijzigdDoor { get; init; }

    [JsonPropertyName("oudeWaarde")]
    public string? OudeWaarde { get; init; }

    [JsonPropertyName("nieuweWaarde")]
    public string? NieuweWaarde { get; init; }

    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }

    [JsonPropertyName("nieuweWaardeExtern")]
    public string? NieuweWaardeExtern { get; init; }
}

public record ZaakDataElement
{
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    [JsonPropertyName("waarde")]
    public string? Waarde { get; init; }

    [JsonPropertyName("datatype")]
    public string? Datatype { get; init; }
}

public record ZaakNotitie
{
    [JsonPropertyName("medewerker")]
    public required string Medewerker { get; init; }

    [JsonPropertyName("datumTijd")]
    public required DateTime DatumTijd { get; init; }

    [JsonPropertyName("notitie")]
    public string? Notitie { get; init; }
}

public record ZaakBetrokkene
{
    [JsonPropertyName("indCorrespondentie")]
    public required bool IndCorrespondentie { get; init; }

    [JsonPropertyName("startdatum")]
    public DateOnly? Startdatum { get; init; }

    [JsonPropertyName("betrokkene")]
    public required Subject Betrokkene { get; init; }

    [JsonPropertyName("typeBetrokkenheid")]
    public required ZaakBetrokkenetype TypeBetrokkenheid { get; init; }

    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }
}

public record ZaakDocument
{
    [JsonPropertyName("functioneleIdentificatie")]
    public required string FunctioneleIdentificatie { get; init; }

    [JsonPropertyName("documenttype")]
    public required ZaakDocumenttype Documenttype { get; init; }

    [JsonPropertyName("documentStatus")]
    public required ZaakDocumentStatus DocumentStatus { get; init; }

    [JsonPropertyName("titel")]
    public required string Titel { get; init; }

    [JsonPropertyName("kenmerk")]
    public string? Kenmerk { get; init; }

    [JsonPropertyName("creatieDatumTijd")]
    public required DateTime CreatieDatumTijd { get; init; }

    [JsonPropertyName("wijzigDatumTijd")]
    public DateTime? WijzigDatumTijd { get; init; }

    [JsonPropertyName("publicatieniveau")]
    public required DocumentPublicatieniveau Publicatieniveau { get; init; }

    [JsonPropertyName("aanvraagDocument")]
    public required bool AanvraagDocument { get; init; }

    [JsonPropertyName("ontvangstDatum")]
    public DateOnly? OntvangstDatum { get; init; }

    [JsonPropertyName("verzendDatum")]
    public DateOnly? VerzendDatum { get; init; }

    [JsonPropertyName("documentrichting")]
    public required DocumentRichting Documentrichting { get; init; }

    [JsonPropertyName("locatie")]
    public string? Locatie { get; init; }

    [JsonPropertyName("beschrijving")]
    public string? Beschrijving { get; init; }

    [JsonPropertyName("documentversies")]
    public required IReadOnlyList<Documentversie> Documentversies { get; init; }

    [JsonPropertyName("historie")]
    public required IReadOnlyList<Documenthistorie> Historie { get; init; }

    [JsonPropertyName("geautoriseerdVoorMedewerkers")]
    public required bool GeautoriseerdVoorMedewerkers { get; init; }

    [JsonPropertyName("converterenNaarPdfa")]
    public required bool ConverterenNaarPdfa { get; init; }

    [JsonPropertyName("locked")]
    public required bool Locked { get; init; }

    [JsonPropertyName("lockEigenaarId")]
    public string? LockEigenaarId { get; init; }

    [JsonPropertyName("lockDatumTijd")]
    public DateTime? LockDatumTijd { get; init; }
}

public record ZaakDocumenttype
{
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    [JsonPropertyName("actief")]
    public required bool Actief { get; init; }

    [JsonPropertyName("documentcategorie")]
    public string? Documentcategorie { get; init; }

    [JsonPropertyName("publicatieniveau")]
    public required DocumentPublicatieniveau Publicatieniveau { get; init; }
}

public record ZaakDocumentStatus
{
    [JsonPropertyName("naam")]
    public required string Naam { get; init; }

    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    [JsonPropertyName("actief")]
    public required bool Actief { get; init; }
}

public record Documentversie
{
    [JsonPropertyName("versienummer")]
    public required int Versienummer { get; init; }

    [JsonPropertyName("documentInhoudID")]
    public required long DocumentInhoudID { get; init; }

    [JsonPropertyName("creatiedatum")]
    public required DateOnly Creatiedatum { get; init; }

    [JsonPropertyName("bestandsnaam")]
    public required string Bestandsnaam { get; init; }

    [JsonPropertyName("mimetype")]
    public required string Mimetype { get; init; }

    [JsonPropertyName("compressed")]
    public required bool Compressed { get; init; }

    [JsonPropertyName("auteur")]
    public string? Auteur { get; init; }

    [JsonPropertyName("afzender")]
    public string? Afzender { get; init; }

    [JsonPropertyName("documentgrootte")]
    public long? Documentgrootte { get; init; }
}

public record Documenthistorie
{
    [JsonPropertyName("typeWijziging")]
    public required DocumentHistorieTypeWijziging TypeWijziging { get; init; }

    [JsonPropertyName("wijzigingDatum")]
    public required DateOnly WijzigingDatum { get; init; }

    [JsonPropertyName("gewijzigdDoor")]
    public string? GewijzigdDoor { get; init; }

    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }
}

public record Betaalgegevens
{
    [JsonPropertyName("transactieId")]
    public string? TransactieId { get; init; }

    [JsonPropertyName("kenmerk")]
    public string? Kenmerk { get; init; }

    [JsonPropertyName("bedrag")]
    public decimal? Bedrag { get; init; }

    [JsonPropertyName("transactieDatum")]
    public DateOnly? TransactieDatum { get; init; }

    [JsonPropertyName("ncerror")]
    public string? Ncerror { get; init; }

    [JsonPropertyName("origineleStatusCode")]
    public string? OrigineleStatusCode { get; init; }

    [JsonPropertyName("betaalstatus")]
    public Betaalstatus? Betaalstatus { get; init; }
}

public record Taak
{
    [JsonPropertyName("afdeling")]
    public string? Afdeling { get; init; }

    [JsonPropertyName("functioneelIdentificatie")]
    public required string FunctioneelIdentificatie { get; init; }

    [JsonPropertyName("groep")]
    public string? Groep { get; init; }

    [JsonPropertyName("behandelaar")]
    public string? Behandelaar { get; init; }

    [JsonPropertyName("startdatum")]
    public required DateOnly Startdatum { get; init; }

    [JsonPropertyName("streefdatum")]
    public DateOnly? Streefdatum { get; init; }

    [JsonPropertyName("fataledatum")]
    public DateOnly? Fataledatum { get; init; }

    [JsonPropertyName("einddatum")]
    public DateTime? Einddatum { get; init; }

    [JsonPropertyName("procesTaak")]
    public string? ProcesTaak { get; init; }

    [JsonPropertyName("indicatieExternToegankelijk")]
    public required bool IndicatieExternToegankelijk { get; init; }

    [JsonPropertyName("afgehandeldDoor")]
    public string? AfgehandeldDoor { get; init; }

    [JsonPropertyName("processtap")]
    public string? Processtap { get; init; }

    [JsonPropertyName("taaktype")]
    public required Taaktype Taaktype { get; init; }

    [JsonPropertyName("taaktypeOrigineel")]
    public required Taaktype TaaktypeOrigineel { get; init; }

    [JsonPropertyName("opschorttermijnStartdatum")]
    public DateOnly? OpschorttermijnStartdatum { get; init; }

    [JsonPropertyName("opschorttermijnEinddatum")]
    public DateOnly? OpschorttermijnEinddatum { get; init; }

    [JsonPropertyName("historie")]
    public required IReadOnlyList<TaakHistorie> Historie { get; init; }

    [JsonPropertyName("toekenningEmail")]
    public string? ToekenningEmail { get; init; }

    [JsonPropertyName("vestigingsnummer")]
    public string? Vestigingsnummer { get; init; }

    [JsonPropertyName("kvkNummer")]
    public string? KvkNummer { get; init; }

    [JsonPropertyName("authenticatieniveau")]
    public string? Authenticatieniveau { get; init; }
}

public record TaakHistorie
{
    [JsonPropertyName("typeWijziging")]
    public required TaakHistorieTypeWijziging TypeWijziging { get; init; }

    [JsonPropertyName("wijzigingDatum")]
    public DateOnly? WijzigingDatum { get; init; }

    [JsonPropertyName("gewijzigdDoor")]
    public string? GewijzigdDoor { get; init; }

    [JsonPropertyName("oudeWaarde")]
    public string? OudeWaarde { get; init; }

    [JsonPropertyName("nieuweWaarde")]
    public string? NieuweWaarde { get; init; }

    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }
}

public record BAGObject
{
    [JsonPropertyName("bagObjectId")]
    public required string BagObjectId { get; init; }
}

public record ZaakZaakKoppeling
{
    [JsonPropertyName("gekoppeldeZaak")]
    public required string GekoppeldeZaak { get; init; }

    [JsonPropertyName("relatietype")]
    public required ZaakRelatietype Relatietype { get; init; }

    [JsonPropertyName("dossierEigenaar")]
    public required bool DossierEigenaar { get; init; }
}

public record Besluit
{
    [JsonPropertyName("functioneleIdentificatie")]
    public required string FunctioneleIdentificatie { get; init; }

    [JsonPropertyName("besluittype")]
    public required Besluittype Besluittype { get; init; }

    [JsonPropertyName("functioneleIdentificatieDocument")]
    public string? FunctioneleIdentificatieDocument { get; init; }

    [JsonPropertyName("documenttype")]
    public Documenttype? Documenttype { get; init; }

    [JsonPropertyName("besluitDatum")]
    public required DateOnly BesluitDatum { get; init; }

    [JsonPropertyName("vervaldatum")]
    public DateOnly? Vervaldatum { get; init; }

    [JsonPropertyName("ingangsdatum")]
    public DateOnly? Ingangsdatum { get; init; }

    [JsonPropertyName("reactiedatum")]
    public DateOnly? Reactiedatum { get; init; }

    [JsonPropertyName("publicatiedatum")]
    public DateOnly? Publicatiedatum { get; init; }

    [JsonPropertyName("berekenVervaldatum")]
    public required bool BerekenVervaldatum { get; init; }

    [JsonPropertyName("toelichting")]
    public string? Toelichting { get; init; }

    [JsonPropertyName("procestermijnInMaanden")]
    public int? ProcestermijnInMaanden { get; init; }
}

public record Besluittype
{
    [JsonPropertyName("naam")]
    public string? Naam { get; init; }

    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }

    [JsonPropertyName("besluitcategorie")]
    public required Besluitcategorie Besluitcategorie { get; init; }

    [JsonPropertyName("reactietermijnInDagen")]
    public required int ReactietermijnInDagen { get; init; }

    [JsonPropertyName("publicatieIndicatie")]
    public required bool PublicatieIndicatie { get; init; }

    [JsonPropertyName("publicatietekst")]
    public string? Publicatietekst { get; init; }

    [JsonPropertyName("publicatietermijnInDagen")]
    public int? PublicatietermijnInDagen { get; init; }
}

public record Besluitcategorie
{
    [JsonPropertyName("naam")]
    public string? Naam { get; init; }

    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }
}

public record Documenttype
{
    [JsonPropertyName("naam")]
    public string? Naam { get; init; }

    [JsonPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }
}

public record ZaakKanaal
{
    [JsonPropertyName("naam")]
    public required Kanaal Naam { get; init; }
}
