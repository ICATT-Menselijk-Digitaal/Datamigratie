namespace Datamigratie.Data.Entities
{
    /// <summary>
    /// Links a DET zaaktype to an OpenZaak zaaktype, independent of its versions.
    /// The versions and their mappings live in <see cref="ZaaktypeVersieMapping"/>.
    /// </summary>
    public class ZaaktypenMapping
    {

        public Guid Id { get; set; }

        public required string DetZaaktypeId { get; set; }

        /// <summary>
        /// Identificatie of the OpenZaak zaaktype; together with <see cref="OzCatalogusId"/> it identifies all its versions.
        /// </summary>
        public required string OzZaaktypeIdentificatie { get; set; }

        /// <summary>
        /// UUID of the OpenZaak catalogus the zaaktype belongs to.
        /// </summary>
        public required Guid OzCatalogusId { get; set; }

        public ICollection<ZaaktypeVersieMapping> Versies { get; set; } = [];
    }
}
