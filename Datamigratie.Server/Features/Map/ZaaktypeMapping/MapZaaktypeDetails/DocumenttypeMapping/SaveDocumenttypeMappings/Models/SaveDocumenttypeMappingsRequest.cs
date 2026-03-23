namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings.Models;

public class SaveDocumenttypeMappingsRequest
{
    public required List<DocumenttypeMappingItem> Mappings { get; set; }
}

public class DocumenttypeMappingItem
{
    public required string DetDocumenttypeNaam { get; set; }
    public required string OzInformatieobjecttypeUrl { get; set; }
}
