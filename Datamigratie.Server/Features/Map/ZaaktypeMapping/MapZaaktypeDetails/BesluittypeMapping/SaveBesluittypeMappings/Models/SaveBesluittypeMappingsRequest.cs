namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Models;

public class SaveBesluittypeMappingsRequest
{
    public required List<BesluittypeMappingItem> Mappings { get; set; }
}

public class BesluittypeMappingItem
{
    public required string DetBesluittypeNaam { get; set; }
    public required Guid OzBesluittypeId { get; set; }
}
