namespace Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Models;

public class StatusMappingsResponse
{
    public required List<StatusMappingDto> Mappings { get; set; }
}

public class StatusMappingDto
{
    public required string DetStatusNaam { get; set; }
    public required Guid OzStatustypeId { get; set; }
}
