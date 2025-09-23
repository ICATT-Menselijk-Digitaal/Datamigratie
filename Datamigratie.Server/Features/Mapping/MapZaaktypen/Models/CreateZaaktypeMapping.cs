namespace Datamigratie.Server.Features.Mapping.MapZaaktypen.Models
{
    public class CreateZaaktypeMapping
    {
        public required string DetZaaktypeId { get; set; }

        public required Guid OzZaaktypeId { get; set; }
    }
}
