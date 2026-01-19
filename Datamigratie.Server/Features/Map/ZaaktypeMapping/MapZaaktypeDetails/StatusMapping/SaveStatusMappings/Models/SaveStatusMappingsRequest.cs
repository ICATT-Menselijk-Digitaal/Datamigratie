namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.StatusMapping.SaveStatusMappings.Models;

public class SaveStatusMappingsRequest
{
    public required List<StatusMappingItem> Mappings { get; set; }
}

public class StatusMappingItem
{
    public required string DetStatusNaam { get; set; }
    public required Guid OzStatustypeId { get; set; }
}
