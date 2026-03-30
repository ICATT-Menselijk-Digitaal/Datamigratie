namespace Datamigratie.Data.Entities;

public class RoltypeMapping
{
    public Guid Id { get; set; }
    public required Guid ZaaktypenMappingId { get; set; }
    public required string DetRol { get; set; }

    /// <summary>
    /// When true, the rol is only included in the generated PDF and not migrated to OpenZaak.
    /// </summary>
    public required bool AlleenPdf { get; set; }

    /// <summary>
    /// URL of the OZ Roltype. Must be NULL when <see cref="AlleenPdf"/> is true.
    /// </summary>
    public string? OzRoltypeUrl { get; set; }

    // Navigation property
    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;
}
