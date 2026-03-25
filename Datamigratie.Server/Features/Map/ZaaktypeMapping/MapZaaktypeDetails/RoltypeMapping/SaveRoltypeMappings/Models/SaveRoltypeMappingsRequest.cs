namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings.Models;

public class SaveRoltypeMappingsRequest
{
    public required List<RoltypeMappingItem> Mappings { get; set; }
}

public class RoltypeMappingItem
{
    public required string DetRol { get; set; }
    public required string OzRoltypeUrl { get; set; }
}
