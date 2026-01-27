using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.SelectDetZaaktypeToMigrate.ShowDetZaaktypeInfo.Models
{
    public class EnrichedDetZaaktype : DetZaaktype
    {
        public required int ClosedZakenCount { get; set; }
        public List<DetStatus> Statuses { get; set; } = [];
        public List<DetDocumenttype> Documenttypen { get; set; } = [];
    }
}
