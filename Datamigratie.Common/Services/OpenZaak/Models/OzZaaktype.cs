using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class OzZaaktype
    {
        public required string Onderwerp { get; set; }
        public required string Identificatie { get; set; }
    }
}
