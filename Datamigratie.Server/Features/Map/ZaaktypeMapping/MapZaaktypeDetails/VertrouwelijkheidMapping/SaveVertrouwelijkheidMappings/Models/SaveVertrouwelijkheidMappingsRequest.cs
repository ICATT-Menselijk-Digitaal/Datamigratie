using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.VertrouwelijkheidMapping.SaveVertrouwelijkheidMappings.Models;

public class SaveVertrouwelijkheidMappingsRequest
{
    public required List<VertrouwelijkheidMappingItem> Mappings { get; set; }
}

public class VertrouwelijkheidMappingItem
{
    public required bool DetVertrouwelijkheid { get; set; }
    public required VertrouwelijkheidsAanduiding OzVertrouwelijkheidaanduiding { get; set; }
}
