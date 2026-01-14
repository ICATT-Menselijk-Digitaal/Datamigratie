using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypeInfo.Models
{
    public class EnrichedOzZaaktype : OzZaaktype
    {
        public List<OzStatustype> Statustypes { get; set; } = [];

        public List<OzResultaattype> Resultaattypen { get; set; } = [];
    }
}
