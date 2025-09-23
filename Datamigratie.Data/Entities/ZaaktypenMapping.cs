namespace Datamigratie.Data.Entities
{
    public class ZaaktypenMapping
    {

        public Guid Id { get; set; }
        public required Guid OzZaaktypeId { get; set; }
        public required string DetZaaktypeId { get; set; }
    }
}
