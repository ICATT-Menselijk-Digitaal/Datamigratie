using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.VertrouwelijkheidMapping.ShowVertrouwelijkheidMappings.Models;

public class VertrouwelijkheidMappingResponse
{
    public required bool DetVertrouwelijkheid { get; set; }
    public required ZaakVertrouwelijkheidaanduiding OzVertrouwelijkheidaanduiding { get; set; }
}
