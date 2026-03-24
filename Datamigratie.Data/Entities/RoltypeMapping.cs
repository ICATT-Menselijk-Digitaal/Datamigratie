namespace Datamigratie.Data.Entities;

public class RoltypeMapping
{
    public Guid Id { get; set; }
    public required Guid ZaaktypenMappingId { get; set; }
    public required string DetRol { get; set; }

    /// <summary>
    /// URL of the OZ Roltype, or the special value "alleen_pdf" meaning
    /// the rol information is only included in the generated PDF.
    /// </summary>
    public required string OzRoltypeUrl { get; set; }

    // Navigation property
    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;
}
