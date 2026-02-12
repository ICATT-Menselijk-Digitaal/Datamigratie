using Datamigratie.Common.Models;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.SelectDetZaaktypeToMigrate.ShowDetZaaktypeInfo.Models
{
    public class EnrichedDetZaaktype : DetZaaktype
    {
        public required int ClosedZakenCount { get; set; }
        public List<DetStatus> Statuses { get; set; } = [];
        public List<DetDocumenttype> Documenttypen { get; set; } = [];
        public List<ZaaktypeOptionItem> PublicatieNiveauOptions { get; set; } = [];
        public List<ZaaktypeOptionItem> DetVertrouwelijkheidOpties { get; set; } = [];
        public List<DetBesluittype> Besluittypen { get; set; } = [];
    }
}
