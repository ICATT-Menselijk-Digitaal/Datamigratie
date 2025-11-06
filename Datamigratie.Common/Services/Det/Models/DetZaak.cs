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

        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTimeOffset CreatieDatumTijd { get; set; }
        public DateOnly? Einddatum { get; set; }
        public string? ExterneIdentificatie { get; set; }
        public DateOnly? Fataledatum { get; set; }
        public bool GeautoriseerdVoorMedewerkers { get; set; }
        public bool Heropend { get; set; }
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
        public required List<DetDocument> Documenten { get; set; }
    }

    public class DetDocument
    {
        public required List<DetDocumentVersie> DocumentVersies { get; set; }
        public string? Kenmerk { get; set; }
        public required string Titel { get; set; }
    }

    public class DetDocumentVersie
    {
        public required int Versienummer { get; set; }
        public required long DocumentInhoudID { get; set; }
        public required string Bestandsnaam { get; set; }
        public required string Mimetype { get; set; }
        public long? Documentgrootte { get; set; }
        public required DateOnly Creatiedatum { get; set; }
    }
}
