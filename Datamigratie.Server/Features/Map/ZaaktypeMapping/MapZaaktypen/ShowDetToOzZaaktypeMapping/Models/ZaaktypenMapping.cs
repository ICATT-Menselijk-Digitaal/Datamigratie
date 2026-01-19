namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping.Models
{
    public class ZaaktypenMapping
    {
        public required Guid Id { get; set; }
        
        public required string DetZaaktypeId { get; set; }

        public required Guid OzZaaktypeId { get; set; }
    }
}
