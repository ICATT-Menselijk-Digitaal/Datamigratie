using System.Runtime.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class OzZaak
    {
        public Guid Uuid { get; set; }

        public string Identificatie { get; set; }

        public string Zaaktype { get; set; }

    }
}
