using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datamigratie.Common.Services.Det.Models
{
    public class DetZaak
    {
        public string AangemaaktDoor { get; set; }
        public string Afdeling { get; set; }
        public DateTimeOffset CreatieDatumTijd { get; set; }
        public DateTime? Einddatum { get; set; }
        public string ExterneIdentificatie { get; set; }
        public DateTime? Fataledatum { get; set; }
        public string FunctioneleIdentificatie { get; set; }
        public bool GeautoriseerdVoorMedewerkers { get; set; }
        public bool Heropend { get; set; }
        public bool Intake { get; set; }
        public bool Notificeerbaar { get; set; }
        public string Omschrijving { get; set; }
        public bool Open { get; set; }
        public bool ProcesGestart { get; set; }
        public DateTime Startdatum { get; set; }
        public DateTime Streefdatum { get; set; }
        public bool Vernietiging { get; set; }
        public bool Vertrouwelijk { get; set; }
        public DateTimeOffset WijzigDatumTijd { get; set; }
        public DetZaaktype Zaaktype { get; set; }
    }
}
