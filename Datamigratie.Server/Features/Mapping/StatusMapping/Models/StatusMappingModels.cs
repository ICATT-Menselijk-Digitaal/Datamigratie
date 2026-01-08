namespace Datamigratie.Server.Features.Mapping.StatusMapping.Models;

public class StatusMappingDto
{
    public required string DetStatusNaam { get; set; }
    public required Guid OzStatustypeId { get; set; }
}

public class SaveStatusMappingsRequest
{
    public required string DetZaaktypeId { get; set; }
    public required List<StatusMappingDto> Mappings { get; set; }
}

public class StatusMappingsResponse
{
    public required List<StatusMappingDto> Mappings { get; set; }
}
