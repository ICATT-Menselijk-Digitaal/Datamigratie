namespace Datamigratie.Data.Entities
{
    public class ResultaattypeMapping
    {
        public Guid Id { get; set; }
        public required string DetZaaktypeId { get; set; }
        public required string DetResultaattypeId { get; set; }
        public required Guid OzZaaktypeId { get; set; }
        public required Guid OzResultaattypeId { get; set; }
    }
}
