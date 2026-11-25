using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.SavePublicatieNiveauMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.SavePublicatieNiveauMappings;

[ApiController]
[Route("api/mappings/versies/{zaaktypeVersieMappingId:guid}/publicatieniveaus")]
public class SavePublicatieNiveauMappingsController(ISavePublicatieNiveauMappingsService savePublicatieNiveauMappingsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SavePublicatieNiveauMappings(
        [FromRoute] Guid zaaktypeVersieMappingId,
        [FromBody] SavePublicatieNiveauMappingsRequest request)
    {
        await savePublicatieNiveauMappingsService.SavePublicatieNiveauMappings(zaaktypeVersieMappingId, request);
        return Ok();
    }
}
