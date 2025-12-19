namespace Datamigratie.Server.Features.Mapping.ZaaktypeMapping.MapZaaktypen.Models
{
    public class UpdateZaaktypeMapping
    {
        public required string DetZaaktypeId { get; set; }

        public required Guid UpdatedOzZaaktypeId { get; set; }
    }
}
