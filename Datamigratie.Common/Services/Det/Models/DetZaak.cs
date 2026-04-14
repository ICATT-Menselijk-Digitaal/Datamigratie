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
        public string? Behandelaar { get; set; }
        public DetBetrokkenePersoon? Initiator { get; set; }
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
        public List<DetDocumentMetadata>? DocumentMetadata { get; set; }
        public required List<DetDocumentHistorie> Historie { get; set; }
        public bool GeautoriseerdVoorMedewerkers { get; set; }

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
        public DetRolType? TypeBetrokkenheid { get; set; }
        public DetBetrokkenePersoon? Betrokkene { get; set; }
    }

    public class DetBetrokkenePersoon
    {
        public DetSubjecttype? Subjecttype { get; set; }
        public string? BurgerServiceNummer { get; set; }
        public string? KvkNummer { get; set; }
        public string? Vestigingsnummer { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter<DetSubjecttype>))]
    public enum DetSubjecttype
    {
        persoon,
        bedrijf,
    }

    public class DetZaakdata
    {
        public required string Type { get; set; }
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public string? Formatering { get; set; }
        public object? Waarde { get; set; }
        public List<string>? Waarden { get; set; }
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
        public string? OverbrengenDoor { get; set; }
        public string? OverbrengenNaar { get; set; }
        public string? OverbrengenType { get; set; }

        public DateOnly? OverbrengenOp { get; set; }

        public string? SelectielijstItemNaam { get; set; }
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
}
