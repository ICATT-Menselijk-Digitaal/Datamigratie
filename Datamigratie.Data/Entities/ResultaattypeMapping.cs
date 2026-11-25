namespace Datamigratie.Data.Entities
{
    public class ResultaattypeMapping
    {
        public Guid Id { get; set; }
        public required Guid ZaaktypeVersieMappingId { get; set; }
        public required string DetResultaattypeNaam { get; set; }
        public required Guid OzResultaattypeId { get; set; }

        // Navigation property
        public ZaaktypeVersieMapping ZaaktypeVersieMapping { get; set; } = null!;
    }
}
