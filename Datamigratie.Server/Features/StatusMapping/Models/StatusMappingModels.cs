namespace Datamigratie.Server.Features.StatusMapping.Models;

public class DetStatusDto
{
    public required string Naam { get; set; }
    public required string Omschrijving { get; set; }
    public bool Eind { get; set; }
}

public class OzStatustypeDto
{
    public required Guid Uuid { get; set; }
    public required string Omschrijving { get; set; }
    public int Volgnummer { get; set; }
    public bool IsEindstatus { get; set; }
}

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
    public required List<DetStatusDto> DetStatuses { get; set; }
    public required List<OzStatustypeDto> OzStatustypes { get; set; }
    public required List<StatusMappingDto> ExistingMappings { get; set; }
}
