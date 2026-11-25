namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping.Models
{
    public class ZaaktypenMapping
    {
        public required Guid Id { get; set; }
        
        public required string DetZaaktypeId { get; set; }

        public required Guid OzZaaktypeId { get; set; }

        /// <summary>
        /// The zaaktype versions, one per selectielijst year, ordered by year. The per-version mappings are saved against their id.
        /// </summary>
        public required List<ZaaktypeVersie> Versies { get; set; }
    }

    public class ZaaktypeVersie
    {
        public required Guid Id { get; set; }

        public required int SelectielijstJaar { get; set; }

        public required Guid OzZaaktypeVersieId { get; set; }
    }
}
