using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.SelectDetZaaktypeToMigrate.ShowDetZaaktypeInfo.Models
{
    public class EnrichedDetZaaktype : DetZaaktype
    {
        public required int ClosedZakenCount { get; set; }
        public List<DetStatus> Statuses { get; set; } = [];
        public List<DetDocumenttype> Documenttypen { get; set; } = [];
        public List<DetPublicatieNiveau> PublicatieNiveauOptions { get; set; } = [];
        public List<DetVertrouwelijkheid> DetVertrouwelijkheidOpties { get; set; } = [];
        public List<DetBesluittype> Besluittypen { get; set; } = [];

    }

    public class DetPublicatieNiveau
    {
        public required string Value { get; set; }
        public required string Label { get; set; }
    }

    public class DetVertrouwelijkheid
    {
        public required string Value { get; set; }
        public required string Label { get; set; }
    }
}
