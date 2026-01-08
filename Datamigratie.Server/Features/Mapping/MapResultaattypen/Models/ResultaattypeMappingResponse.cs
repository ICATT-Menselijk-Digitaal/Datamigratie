namespace Datamigratie.Server.Features.Mapping.Models
{
    public class ResultaattypeMappingResponse
    {
        public required string DetZaaktypeId { get; set; }
        public required string DetResultaattypeId { get; set; }
        public required Guid OzZaaktypeId { get; set; }
        public required Guid OzResultaattypeId { get; set; }
    }
}
