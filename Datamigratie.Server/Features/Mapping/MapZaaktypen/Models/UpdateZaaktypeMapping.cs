namespace Datamigratie.Server.Features.Mapping.MapZaaktypen.Models
{
    public class UpdateZaaktypeMapping
    {
        public required string DetZaaktypeId { get; set; }

        public required Guid OzZaaktypeId { get; set; }
    }
}
