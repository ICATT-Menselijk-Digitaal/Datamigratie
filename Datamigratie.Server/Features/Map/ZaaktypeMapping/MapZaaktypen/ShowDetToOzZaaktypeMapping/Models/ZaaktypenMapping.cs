namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping.Models
{
    public class ZaaktypenMapping
    {
        public required Guid Id { get; set; }
        
        public required string DetZaaktypeId { get; set; }

        /// <summary>
        /// Identificatie of the OZ zaaktype; together with <see cref="OzCatalogusId"/> it identifies all its versions.
        /// </summary>
        public required string OzZaaktypeIdentificatie { get; set; }

        public required Guid OzCatalogusId { get; set; }

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
