using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/roltypen")]
public class SaveRoltypeMappingsController(ISaveRoltypeMappingsService saveRoltypeMappingsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveRoltypeMappings(
        [FromRoute] Guid zaaktypenMappingId,
        [FromBody] SaveRoltypeMappingsRequest request)
    {
        await saveRoltypeMappingsService.SaveRoltypeMappings(zaaktypenMappingId, request);
        return Ok();
    }
}
