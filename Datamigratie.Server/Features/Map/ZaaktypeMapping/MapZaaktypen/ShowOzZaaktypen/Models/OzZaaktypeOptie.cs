namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowOzZaaktypen.Models
{
    /// <summary>
    /// One OZ zaaktype in the zaaktype dropdown, regardless of how many versions it has.
    /// </summary>
    public class OzZaaktypeOptie
    {
        /// <summary>
        /// UUID of the newest published version. Any version of the zaaktype can be used to create a mapping.
        /// </summary>
        public required Guid Id { get; set; }

        public required string Identificatie { get; set; }

        public required string Omschrijving { get; set; }

        /// <summary>
        /// Together with <see cref="Identificatie"/> it identifies the zaaktype, matching the zaaktype mapping.
        /// </summary>
        public required Guid CatalogusId { get; set; }
    }
}
