using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datamigratie.Common.Services.Det.Models
{
    public class DetZaaktype
    {
        public bool Actief { get; set; }
        public required string Naam { get; set; }
        public required string Omschrijving { get; set; }
        public required string FunctioneleIdentificatie { get; set; }
    }
}
