using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.Zaaktypen.ShowZaaktype.Models
{
    public class EnrichedOzZaaktype : OzZaaktype
    {
        public List<OzResultaattype>? Resultaattypen { get; set; }
    }
}
