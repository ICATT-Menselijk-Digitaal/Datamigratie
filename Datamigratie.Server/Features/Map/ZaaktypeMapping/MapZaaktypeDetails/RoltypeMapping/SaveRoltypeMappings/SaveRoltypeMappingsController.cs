using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings;

[ApiController]
[Route("api/mappings/versies/{zaaktypeVersieMappingId:guid}/roltypen")]
public class SaveRoltypeMappingsController(ISaveRoltypeMappingsService saveRoltypeMappingsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveRoltypeMappings(
        [FromRoute] Guid zaaktypeVersieMappingId,
        [FromBody] SaveRoltypeMappingsRequest request)
    {
        await saveRoltypeMappingsService.SaveRoltypeMappings(zaaktypeVersieMappingId, request);
        return Ok();
    }
}
