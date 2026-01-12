namespace Datamigratie.Server.Features.Mapping.StatusMapping.SaveStatusMappings.Models;

public class SaveStatusMappingsRequest
{
    public required string DetZaaktypeId { get; set; }
    public required List<StatusMappingItem> Mappings { get; set; }
}

public class StatusMappingItem
{
    public required string DetStatusNaam { get; set; }
    public required Guid OzStatustypeId { get; set; }
}
