using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.Shared.SelectDetZaaktype.ShowDetZaaktypeInfo.Models
{
    public class EnrichedDetZaaktype : DetZaaktype
    {
        public required int ClosedZakenCount { get; set; }
        public List<DetStatus> Statuses { get; set; } = [];
    }
}
