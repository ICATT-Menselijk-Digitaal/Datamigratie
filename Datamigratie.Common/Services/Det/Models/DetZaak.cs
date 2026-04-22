using System.Text.Json.Serialization;
using Datamigratie.Common.Converters;

namespace Datamigratie.Common.Services.Det.Models
{

    public class DetZaakMinimal
    {
        public required string FunctioneleIdentificatie { get; set; }
        public bool Open { get; set; }
    }

    public class DetZaak : DetZaakMinimal
    {
        public string? AangemaaktDoor { get; set; }
        public string? Afdeling { get; set; }
        public string? Groep { get; set; }

        public string? Behandelaar { get; set; }
        public DetBetaalgegevens? Betaalgegevens { get; set; }

        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTimeOffset CreatieDatumTijd { get; set; }
        public DateOnly? Einddatum { get; set; }
        public string? ExterneIdentificatie { get; set; }
        public DateOnly? Fataledatum { get; set; }
        public bool GeautoriseerdVoorMedewerkers { get; set; }
        public bool Heropend { get; set; }
        public DetResultaat? Resultaat { get; set; }
        public DetStatus? ZaakStatus { get; set; }
        public bool Intake { get; set; }
        public bool Notificeerbaar { get; set; }
        public required string Omschrijving { get; set; }
        public bool ProcesGestart { get; set; }
        public DateOnly Startdatum { get; set; }
        public DateOnly Streefdatum { get; set; }
        public bool Vernietiging { get; set; }
        public bool Vertrouwelijk { get; set; }
        public string? Organisatie { get; set; }
        public DateOnly? OpschorttermijnStartdatum { get; set; }
        public DateOnly? OpschorttermijnEinddatum { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTimeOffset? WijzigDatumTijd { get; set; }
        public DetZaaktype? Zaaktype { get; set; }
        public List<DetDocument>? Documenten { get; set; }
        public List<DetBesluit>? Besluiten { get; set; }
        public string? RedenStart { get; set; }
        public DetArchiveerGegevens? ArchiveerGegevens { get; set; }
        public DetGeolocatie? Geolocatie { get; set; }
        public DetKanaal? Kanaal { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTimeOffset? Ztc1MigratiedatumTijd { get; set; }
        public List<DetZaakNotitie>? Notities { get; set; }
        public List<DetBetrokkene>? Betrokkenen { get; set; }
        public List<DetZaakZaakKoppeling>? GekoppeldeZaken { get; set; }
        public List<DetZaakdata>? Zaakdata { get; set; }
        public List<DetTaak>? Taken { get; set; }
        public List<string>? Contacten { get; set; }
        public List<DetContact>? GekoppeldeContacten { get; set; }
        public List<DetBagObject>? BagObjecten { get; set; }
        public DetSubject? Initiator { get; set; }
        public required List<DetZaakHistorie> Historie { get; set; } = [];
    }

    public class DetDocument
    {
        public required List<DetDocumentVersie> DocumentVersies { get; set; }
        public string? Kenmerk { get; set; }
        public required string Titel { get; set; }
        public string? Beschrijving { get; set; }
        public DocumentTaal? Taal { get; set; }
        public DocumentVorm? DocumentVorm { get; set; }
        public required DetDocumentstatus Documentstatus { get; set; }
        public required string Publicatieniveau { get; set; }
        public DetDocumenttype? Documenttype { get; set; }
        public long? PdfaDocumentInhoudID { get; set; }
        public DetDocumentVersie? PdfaDocumentversie { get; set; }
        public string? DocumentVersturen { get; set; }
        public string? Locatie { get; set; }
        public required bool AanvraagDocument { get; set; }
        public required bool GeautoriseerdVoorMedewerkers { get; set; }
        public required string Documentrichting { get; set; }
        public DateOnly? DocumentVersturenDatum { get; set; }
        public DateOnly? OntvangstDatum { get; set; }
        public DateOnly? VerzendDatum { get; set; }
        public List<DetDocumentPublicatie>? Publicaties { get; set; }
        public List<DetDocumentMetadata>? DocumentMetadata { get; set; }
        public required List<DetDocumentHistorie> Historie { get; set; }
    }

    public class DetDocumentPublicatie
    {
        public required string Bestemming { get; set; }
        public required DateOnly Publicatiedatum { get; set; }
    }

    public class DetDocumentMetadata
    {
        public string? Waarde { get; set; }
        public DetMetadataElement? MetadataElement { get; set; }
    }

    public class DetMetadataElement
    {
        public string? Naam { get; set; }
        public string? Label { get; set; }
        public string? Type { get; set; }
    }

    public class DetDocumentHistorie
    {
        public string? TypeWijziging { get; set; }
        public string? GewijzigdDoor { get; set; }
        public DateOnly? WijzigingDatum { get; set; }
        public string? OudeWaarde { get; set; }
        public string? NieuweWaarde { get; set; }
        public string? Toelichting { get; set; }
    }

    public class DetDocumentVersie
    {
        public required int Versienummer { get; set; }
        public required long DocumentInhoudID { get; set; }
        public required string Bestandsnaam { get; set; }
        public required string Mimetype { get; set; }
        public required bool Compressed { get; set; }
        public long? Documentgrootte { get; set; }
        public string? Auteur { get; set; }
        public required DateOnly Creatiedatum { get; set; }
        public List<DetOndertekening>? Ondertekeningen { get; set; }
        public string? Afzender { get; set; }
    }

    public class DetOndertekening
    {
        public required string Ondertekenaar { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTimeOffset OndertekenDatum { get; set; }
        public required bool Gemandateerd { get; set; }
        public string? Opmerking { get; set; }
        public required string DocumentTitel { get; set; }
        public required DateOnly CreatieDatum { get; set; }
    }

    public class DetZaakNotitie
    {
        public required string Medewerker { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public required DateTimeOffset DatumTijd { get; set; }
        public string? Notitie { get; set; }
    }

    public class DetBetrokkene
    {
        public required bool IndCorrespondentie { get; set; }
        public DateOnly? Startdatum { get; set; }
        public required DetSubject Betrokkene { get; set; }
        public required DetRolType TypeBetrokkenheid { get; set; }
        public string? Toelichting { get; set; }
    }

    public class DetTaak
    {
        public required string FunctioneelIdentificatie { get; set; }
        public required DateOnly Startdatum { get; set; }
        public required bool IndicatieExternToegankelijk { get; set; }
        public required string Taaktype { get; set; }
        public required List<DetTaakHistorie> Historie { get; set; }
        public string? Processtap { get; set; }
        public string? AfgehandeldDoor { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTimeOffset? Einddatum { get; set; }
        public DateOnly? Fataledatum { get; set; }
        public DateOnly? Streefdatum { get; set; }
        public string? Vestigingsnummer { get; set; }
        public string? KvkNummer { get; set; }
        public string? ToekenningEmail { get; set; }
    }

    public class DetTaakHistorie
    {
        public required string TypeWijziging { get; set; }
        public string? GewijzigdDoor { get; set; }
        public DateOnly? WijzigingDatum { get; set; }
        public string? OudeWaarde { get; set; }
        public string? NieuweWaarde { get; set; }
        public string? Toelichting { get; set; }
    }

    public class DetZaakHistorie
    {
        public required string TypeWijziging { get; set; }
        public string? GewijzigdDoor { get; set; }
        public DateOnly? WijzigingDatum { get; set; }
        public string? OudeWaarde { get; set; }
        public string? NieuweWaarde { get; set; }
        public string? NieuweWaardeExtern { get; set; }
        public string? Toelichting { get; set; }
    }

    public class DetBetaalgegevens
    {
        public decimal? Bedrag { get; set; }
        public string? Betaalstatus { get; set; }
        public string? Kenmerk { get; set; }
        public DateOnly? TransactieDatum { get; set; }
        public string? OrigineleStatusCode { get; set; }
        public string? TransactieId { get; set; }
        public string? Ncerror { get; set; }
    }

    public class DetArchiveerGegevens
    {
        public DateOnly? BewaartermijnEinddatum { get; set; }
        public string? BewaartermijnWaardering { get; set; }
        public DateOnly? OverbrengenOp { get; set; }
        public string? OverbrengenDoor { get; set; }
        public string? OverbrengenNaar { get; set; }
        public string? OverbrengenType { get; set; }
        public string? SelectielijstItemNaam { get; set; }
        public string? ZaaktypeNaam { get; set; }
        public DetOvergebrachteGegevens? OvergebrachteGegevens { get; set; }
    }

    public class DetOvergebrachteGegevens
    {
        public required DateOnly OvergebrachtOp { get; set; }
        public required string OvergebrachtDoor { get; set; }
        public required string OvergebrachtNaar { get; set; }
    }

    public class DocumentTaal
    {
        public required string FunctioneelId { get; set; }
        public required string Naam { get; set; }
    }

    public class DocumentVorm
    {
        public required string Naam { get; set; }
    }

    public class DetBesluit
    {
        public required string FunctioneleIdentificatie { get; set; }
        public required DetBesluittype Besluittype { get; set; }
        public required DateOnly BesluitDatum { get; set; }
        public DateOnly? Vervaldatum { get; set; }
        public DateOnly? Ingangsdatum { get; set; }
        public DateOnly? Reactiedatum { get; set; }
        public DateOnly? Publicatiedatum { get; set; }
        public string? Toelichting { get; set; }
        public int? ProcestermijnInMaanden { get; set; }

    }

    public class DetBesluittype
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public bool Actief { get; set; }
    }

    public class DetGeolocatie
    {
        public string? Type { get; set; }
        public List<decimal>? Point2D { get; set; }
    }

    public class DetKanaal
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
    }

    public class DetZaakZaakKoppeling
    {
        public required string GekoppeldeZaak { get; set; }
        public required string Relatietype { get; set; }
        public required bool DossierEigenaar { get; set; }
    }

    public class DetBagObject
    {
        public required string BagObjectId { get; set; }
    }

    public class DetContact
    {
        public required string FunctioneleIdentificatie { get; set; }
        public required bool IndicatieVertrouwelijk { get; set; }
        public string? Emailadres { get; set; }
        public string? Telefoonnummer { get; set; }
        public string? TelefoonnummerAlternatief { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public required DateTimeOffset StartdatumTijd { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTimeOffset? EinddatumTijd { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTimeOffset? StreefdatumTijd { get; set; }
        public string? Vraag { get; set; }
        public string? Antwoord { get; set; }
        public DetContacttype? Type { get; set; }
        public DetContactstatus? Status { get; set; }
        public DetContactprioriteit? Prioriteit { get; set; }
        public List<DetContactHistorie>? Historie { get; set; }
        public List<DetVoorlopigAntwoord>? VoorlopigeAntwoorden { get; set; }
        public DetKanaal? Kanaal { get; set; }
        public DetSubject? Aanvrager { get; set; }
        public List<DetBagObject>? BagObjecten { get; set; }
        public List<string>? GekoppeldeContacten { get; set; }
    }

    public class DetVoorlopigAntwoord
    {
        public required string Antwoord { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public required DateTimeOffset AntwoordDatumTijd { get; set; }
    }

    public class DetContacttype
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
    }

    public class DetContactstatus
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
    }

    public class DetContactprioriteit
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public required int Dagen { get; set; }
    }

    public class DetContactHistorie
    {
        public string? GewijzigdDoor { get; set; }
        public string? TypeWijziging { get; set; }
        public string? OudeWaarde { get; set; }
        public string? NieuweWaarde { get; set; }

        public string? Toelichting { get; set; }
        public DateOnly? WijzigingDatum { get; set; }
    }

    public class DetContactKanaal
    {
        public string? Omschrijving { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter<DetSubjecttype>))]
    public enum DetSubjecttype
    {
        persoon,
        bedrijf,
    }

    [JsonConverter(typeof(DetSubjectConverter))]
    public abstract class DetSubject
    {
        public long? Identifier { get; set; }
        public DetSubjecttype? Subjecttype { get; set; }
        public string? Telefoonnummer { get; set; }
        public string? TelefoonnummerAlternatief { get; set; }
        public string? Rekeningnummer { get; set; }
        public string? Emailadres { get; set; }
        public bool? OntvangenZaakNotificaties { get; set; }
        public bool? ToestemmingZaakNotificatiesAlleenDigitaal { get; set; }
        public required bool HandmatigToegevoegd { get; set; }
        public List<DetSubjectNotitie>? Notities { get; set; }
        public List<DetAdres>? Adressen { get; set; }
    }

    public class DetPersoon : DetSubject
    {
        public string? BurgerServiceNummer { get; set; }
        public string? Voornamen { get; set; }
        public string? Voorletters { get; set; }
        public string? GeslachtsNaam { get; set; }
        public string? Voorvoegsel { get; set; }
        public string? Geslacht { get; set; }
        public string? AanhefAanschrijving { get; set; }
        public string? AdelijkeTitel { get; set; }
        public string? PreAcademischeTitel { get; set; }
        public string? PostAcademischeTitel { get; set; }
        public string? Naamgebruik { get; set; }
        public string? GeslachtsNaamPartner { get; set; }
        public string? VoorvoegselPartner { get; set; }
        public DateOnly? Geboortedatum { get; set; }
        public string? Geboorteplaats { get; set; }
        public DateOnly? Overlijdensdatum { get; set; }
        public string? Overlijdensplaats { get; set; }
        public string? ANummer { get; set; }
        public string? OpschortingsReden { get; set; }
        public DateOnly? OpschortingsDatum { get; set; }
        public required bool Geblokkeerd { get; set; }
        public required bool CurateleRegister { get; set; }
        public required bool InOnderzoek { get; set; }
        public string? GeboortedatumVolledig { get; set; }
        public string? OverlijdensdatumVolledig { get; set; }
        public required bool BeperkingVerstrekking { get; set; }
        public string? NietIngezeteneAanduiding { get; set; }
        public required bool AfnemerIndicatie { get; set; }
        public long? AnpIdentificatie { get; set; }
        public string? Gemeentecode { get; set; }
        public DetLand? Geboorteland { get; set; }
        public DetLand? Overlijdensland { get; set; }
        public DetBurgerlijkeStaat? BurgerlijkeStaat { get; set; }
        public List<DetNationaliteit>? Nationaliteiten { get; set; }
        public List<DetReisdocument>? Reisdocumenten { get; set; }
        public List<DetRelatie>? Relaties { get; set; }
    }

    public class DetBedrijf : DetSubject
    {
        public string? KvkNummer { get; set; }
        public string? Vestigingsnummer { get; set; }
        public string? BuitenlandsHandelsregisternummer { get; set; }
        public string? Bedrijfsnaam { get; set; }
        public string? Vennootschapsnaam { get; set; }
        public string? StatutaireZetel { get; set; }
        public DateOnly? DatumVestiging { get; set; }
        public DateOnly? DatumOpheffing { get; set; }
        public DateOnly? DatumVoortzetting { get; set; }
        public string? Faxnummer { get; set; }
        public int? AantalWerknemers { get; set; }
        public required bool InSurceance { get; set; }
        public required bool Failliet { get; set; }
        public string? Rsinummer { get; set; }
        public string? Vestigingsstatus { get; set; }
        public required DateOnly Ingangsdatum { get; set; }
        public DateOnly? Mutatiedatum { get; set; }
        public required string Vestigingstype { get; set; }
        public DetHoofdactiviteit? Hoofdactiviteit { get; set; }
        public DetRechtsvorm? Rechtsvorm { get; set; }
        public List<DetNevenactiviteit>? Nevenactiviteiten { get; set; }
        public List<DetContactpersoon>? Contactpersonen { get; set; }
    }

    public class DetAdres
    {
        public required string Type { get; set; }
        public string? Straatnaam { get; set; }
        public string? Postcode { get; set; }
        public string? Plaatsnaam { get; set; }
        public string? Huisletter { get; set; }
        public int? Huisnummer { get; set; }
        public string? Huisnummertoevoeging { get; set; }
        public string? Huisnummeraanduiding { get; set; }
        public string? Adresbuitenland1 { get; set; }
        public string? Adresbuitenland2 { get; set; }
        public string? Adresbuitenland3 { get; set; }
        public required bool BuitenlandsAdres { get; set; }
        public required DetLand Land { get; set; }
    }

    public class DetLand
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public required bool Actief { get; set; }
        public required string GbaCode { get; set; }
    }

    public class DetBurgerlijkeStaat
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public required bool Actief { get; set; }
        public required string GbaCode { get; set; }
    }

    public class DetNationaliteit
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public required bool Actief { get; set; }
        public required string GbaCode { get; set; }
        public string? RedenVerkrijging { get; set; }
        public DateOnly? DatumVerkrijging { get; set; }
    }

    public class DetReisdocument
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public required bool Actief { get; set; }
        public required string GbaCode { get; set; }
        public required bool IndicatieOnttrekking { get; set; }
        public string? AutoriteitOntrekking { get; set; }
        public string? IndicatieVervallen { get; set; }
        public string? AutoriteitVervallen { get; set; }
        public DateOnly? EinddatumGeldigheid { get; set; }
        public string? Reisdocumentnummer { get; set; }
        public DateOnly? Uitgiftedatum { get; set; }
        public string? AutoriteitUitgifte { get; set; }
    }

    public class DetRelatie
    {
        public required string Type { get; set; }
        public string? SoortVerbintenis { get; set; }
        public DateOnly? DatumSluitingVerbintenis { get; set; }
        public string? PlaatsSluitingVerbintenis { get; set; }
        public DetLand? LandSluitingVerbintenis { get; set; }
        public DateOnly? DatumOntbindingVerbintenis { get; set; }
        public string? RedenOntbindingVerbintenis { get; set; }
        public string? PlaatsOntbindingVerbintenis { get; set; }
        public DetLand? LandOntbindingVerbintenis { get; set; }
        public long? IdentifierPersoon { get; set; }
    }

    public class DetRechtsvorm
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public required bool Actief { get; set; }
        public required string Code { get; set; }
        public string? NaamNhr { get; set; }
    }

    public class DetHoofdactiviteit
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public required bool Actief { get; set; }
        public required string Code { get; set; }
    }

    public class DetNevenactiviteit
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public required bool Actief { get; set; }
        public string? Code { get; set; }
    }

    public class DetContactpersoon
    {
        public string? Naam { get; set; }
        public string? Geslacht { get; set; }
        public string? Emailadres { get; set; }
        public string? Telefoonnummer { get; set; }
        public string? Faxnummer { get; set; }
        public string? Functie { get; set; }
    }

    public class DetSubjectNotitie
    {
        public DateOnly? IngangsdatumGeldigheid { get; set; }
        public string? Afdeling { get; set; }
        public string? Groep { get; set; }
        public DateOnly? EinddatumGeldigheid { get; set; }
        public required DateOnly AangemaaktOp { get; set; }
        public required string AangemaaktDoor { get; set; }
        public required string Titel { get; set; }
        public required string Inhoud { get; set; }
    }
}
