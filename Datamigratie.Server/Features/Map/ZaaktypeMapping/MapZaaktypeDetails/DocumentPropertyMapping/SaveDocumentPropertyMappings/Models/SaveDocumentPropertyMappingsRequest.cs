namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.SaveDocumentPropertyMappings.Models;

public class SaveDocumentPropertyMappingsRequest
{
    public required List<DocumentPropertyMappingItem> Mappings { get; set; }
}

public class DocumentPropertyMappingItem
{
    public required string DetPropertyName { get; set; }
    
    public required string DetValue { get; set; }
    
    public required string OzValue { get; set; }
}
