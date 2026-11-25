namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowOzZaaktypen.Models
{
    /// <summary>
    /// One OZ zaaktype in the zaaktype dropdown, regardless of how many versions it has.
    /// </summary>
    public class OzZaaktypeOptie
    {
        public required string Identificatie { get; set; }

        public required string Omschrijving { get; set; }

        /// <summary>
        /// Together with <see cref="Identificatie"/> it identifies the zaaktype; both are sent to create a zaaktype mapping.
        /// </summary>
        public required Guid CatalogusId { get; set; }
    }
}
