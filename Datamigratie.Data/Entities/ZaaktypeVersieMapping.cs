namespace Datamigratie.Data.Entities;

/// <summary>
/// One OpenZaak zaaktype version a DET zaaktype is mapped against: the newest published version per selectielijst year.
/// The per-version mappings (status, resultaattype, roltype, ...) hang off this entity.
/// </summary>
public class ZaaktypeVersieMapping
{
    public Guid Id { get; set; }

    public required Guid ZaaktypenMappingId { get; set; }

    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;

    /// <summary>
    /// Year of the selectielijst procestype of this zaaktype version (e.g. 2017, 2020).
    /// Null for rows carried over from before mapping per version existed, until it is looked up in OpenZaak.
    /// </summary>
    public int? SelectielijstJaar { get; set; }

    /// <summary>
    /// UUID of the OpenZaak zaaktype version the mappings were made against.
    /// A newer published version for the same year has a different UUID, which signals the mappings need review.
    /// </summary>
    public required Guid OzZaaktypeVersieId { get; set; }
}
