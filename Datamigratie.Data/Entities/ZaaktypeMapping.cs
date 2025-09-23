namespace Datamigratie.Data.Entities
{
    public class ZaaktypeMapping
    {

        public Guid Id { get; set; }
        public required Guid OzZaaktypeId { get; set; }
        public required string DetZaaktypeId { get; set; }
    }
}
