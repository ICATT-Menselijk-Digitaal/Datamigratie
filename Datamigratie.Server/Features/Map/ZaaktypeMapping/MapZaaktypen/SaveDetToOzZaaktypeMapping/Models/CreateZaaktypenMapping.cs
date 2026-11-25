namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping.Models
{
    public class CreateZaaktypenMapping
    {
        /// <summary>
        /// Identificatie of the OZ zaaktype; together with <see cref="OzCatalogusId"/> it identifies all its versions.
        /// </summary>
        public required string OzZaaktypeIdentificatie { get; set; }

        public required Guid OzCatalogusId { get; set; }
    }
}
