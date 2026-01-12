namespace Datamigratie.Server.Features.Mapping.ZaaktypeMapping.Models
{
    public class ZaaktypenMapping
    {
        public required Guid Id { get; set; }
        
        public required string DetZaaktypeId { get; set; }

        public required Guid OzZaaktypeId { get; set; }
    }
}
