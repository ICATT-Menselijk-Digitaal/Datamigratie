using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.ShowOzZaaktypeInfo.Models
{
    public class EnrichedOzZaaktype : OzZaaktype
    {
        public List<OzStatustype> Statustypes { get; set; } = [];

        public List<OzResultaattype> Resultaattypen { get; set; } = [];

        public List<OzInformatieobjecttype> Informatieobjecttypen { get; set; } = [];

        public List<OzBesluittype> Besluittypen { get; set; } = [];
    }
}
