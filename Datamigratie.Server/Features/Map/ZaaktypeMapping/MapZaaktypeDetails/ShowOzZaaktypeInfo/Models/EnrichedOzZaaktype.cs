using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Shared.Models;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.ShowOzZaaktypeInfo.Models
{
    public class EnrichedOzZaaktype : OzZaaktype
    {
        public List<OzStatustype> Statustypes { get; set; } = [];

        public List<OzResultaattype> Resultaattypen { get; set; } = [];

        public List<OzInformatieobjecttype> Informatieobjecttypen { get; set; } = [];

        public List<OzBesluittype> Besluittypen { get; set; } = [];

        public List<ZaaktypeOptionItem> OzZaakVertrouwelijkheidaanduidingen { get; set; } = [];

        public List<ZaaktypeOptionItem> OzDocumentVertrouwelijkheidaanduidingen { get; set; } = [];
    }
}
