namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.SavePublicatieNiveauMappings.Models;

public class SavePublicatieNiveauMappingsRequest
{
    public required List<PublicatieNiveauMappingItem> Mappings { get; set; }
}

public class PublicatieNiveauMappingItem
{
    public required string DetPublicatieNiveau { get; set; }
    public required string OzVertrouwelijkheidaanduiding { get; set; }
}
