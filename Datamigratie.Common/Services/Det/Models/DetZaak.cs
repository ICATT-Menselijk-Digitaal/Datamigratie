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
        public DateTimeOffset WijzigDatumTijd { get; set; }
        public DetZaaktype? Zaaktype { get; set; }
        public List<DetDocument>? Documenten { get; set; }
        public List<DetBesluit>? Besluiten { get; set; }
    }

    public class DetDocument
    {
        public required List<DetDocumentVersie> DocumentVersies { get; set; }
        public string? Kenmerk { get; set; }
        public required string Titel { get; set; }
        public string? Beschrijving {get; set; }
        public DocumentTaal? Taal { get; set; }
        public DocumentVorm? DocumentVorm { get; set; }

        public required DetDocumentstatus Documentstatus { get; set; }
        public string? Publicatieniveau { get; set; }
        public DetDocumenttype? Documenttype { get; set; }
    }

    public class DetDocumentVersie
    {
        public required int Versienummer { get; set; }
        public required long DocumentInhoudID { get; set; }
        public required string Bestandsnaam { get; set; }
        public string Mimetype { get; set; }
        public long? Documentgrootte { get; set; }
        public string? Auteur { get; set; }
        public required DateOnly Creatiedatum { get; set; }
    }

    public class DetBetaalgegevens
    {
        public decimal? Bedrag { get; set; }
        public string? Betaalstatus { get; set; }
        public string? Kenmerk { get; set; }
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
        public required DetBesluitBesluittype Besluittype { get; set; }
        public required DateOnly BesluitDatum { get; set; }
        public DateOnly? Vervaldatum { get; set; }
        public DateOnly? Ingangsdatum { get; set; }
        public DateOnly? Reactiedatum { get; set; }
        public DateOnly? Publicatiedatum { get; set; }
        public string? Toelichting { get; set; }
    }

    public class DetBesluitBesluittype
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public bool Actief { get; set; }
    }
}
