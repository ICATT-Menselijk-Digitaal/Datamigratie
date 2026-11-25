using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings;

[ApiController]
[Route("api/mappings/versies/{zaaktypeVersieMappingId:guid}/documenttypen")]
public class SaveDocumenttypeMappingsController(ISaveDocumenttypeMappingsService saveDocumenttypeMappingsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveDocumenttypeMappings(
        [FromRoute] Guid zaaktypeVersieMappingId,
        [FromBody] SaveDocumenttypeMappingsRequest request)
    {
        await saveDocumenttypeMappingsService.SaveDocumenttypeMappings(zaaktypeVersieMappingId, request);
        return Ok();
    }
}
