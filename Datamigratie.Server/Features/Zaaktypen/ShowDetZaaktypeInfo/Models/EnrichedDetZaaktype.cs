using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo.Models
{
    public class EnrichedDetZaaktype : DetZaaktype
    {
        public required int ClosedZaken { get; set; }
    }
}
