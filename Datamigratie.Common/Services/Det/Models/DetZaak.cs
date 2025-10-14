using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
        public DateTime CreatieDatumTijd { get; set; }
        public DateTime? Einddatum { get; set; }
        public string? ExterneIdentificatie { get; set; }
        public DateTime? Fataledatum { get; set; }
        public bool GeautoriseerdVoorMedewerkers { get; set; }
        public bool Heropend { get; set; }
        public bool Intake { get; set; }
        public bool Notificeerbaar { get; set; }
        public required string Omschrijving { get; set; }
        public bool ProcesGestart { get; set; }
        public DateTime Startdatum { get; set; }
        public DateTime Streefdatum { get; set; }
        public bool Vernietiging { get; set; }
        public bool Vertrouwelijk { get; set; }
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTime WijzigDatumTijd { get; set; }
        public DetZaaktype? Zaaktype { get; set; }
    }
}
