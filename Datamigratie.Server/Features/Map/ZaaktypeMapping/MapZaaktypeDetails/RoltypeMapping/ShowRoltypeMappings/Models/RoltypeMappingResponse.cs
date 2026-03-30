namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings.Models;

public class RoltypeMappingResponse
{
    public required string DetRol { get; set; }

    /// <summary>
    /// When true, the rol is only included in the generated PDF and not migrated to OpenZaak.
    /// </summary>
    public required bool AlleenPdf { get; set; }

    /// <summary>
    /// URL of the OZ Roltype. Null when <see cref="AlleenPdf"/> is true.
    /// </summary>
    public string? OzRoltypeUrl { get; set; }
}
