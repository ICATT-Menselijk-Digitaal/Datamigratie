namespace Datamigratie.Data.Entities
{
    /// <summary>
    /// Links a DET zaaktype to an OpenZaak zaaktype, independent of its versions.
    /// The versions and their mappings live in <see cref="ZaaktypeVersieMapping"/>.
    /// </summary>
    public class ZaaktypenMapping
    {

        public Guid Id { get; set; }

        /// <summary>
        /// UUID of the OpenZaak zaaktype version picked in the zaaktype dropdown.
        /// Kept during the transition to mapping per version; to be replaced by
        /// <see cref="OzZaaktypeIdentificatie"/> + <see cref="OzCatalogusId"/>.
        /// </summary>
        public required Guid OzZaaktypeId { get; set; }

        public required string DetZaaktypeId { get; set; }

        /// <summary>
        /// Identificatie of the OpenZaak zaaktype; together with <see cref="OzCatalogusId"/> it identifies all its versions.
        /// Null for mappings created before mapping per version existed, until it is looked up in OpenZaak.
        /// </summary>
        public string? OzZaaktypeIdentificatie { get; set; }

        /// <summary>
        /// UUID of the OpenZaak catalogus the zaaktype belongs to.
        /// Null for mappings created before mapping per version existed, until it is looked up in OpenZaak.
        /// </summary>
        public Guid? OzCatalogusId { get; set; }

        public ICollection<ZaaktypeVersieMapping> Versies { get; set; } = [];
    }
}
