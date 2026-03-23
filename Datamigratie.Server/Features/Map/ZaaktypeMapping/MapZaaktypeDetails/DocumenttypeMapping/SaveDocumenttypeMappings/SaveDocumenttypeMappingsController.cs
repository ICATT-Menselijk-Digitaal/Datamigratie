using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/documenttypen")]
public class SaveDocumenttypeMappingsController(ISaveDocumenttypeMappingsService saveDocumenttypeMappingsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveDocumenttypeMappings(
        [FromRoute] Guid zaaktypenMappingId,
        [FromBody] SaveDocumenttypeMappingsRequest request)
    {
        await saveDocumenttypeMappingsService.SaveDocumenttypeMappings(zaaktypenMappingId, request);
        return Ok();
    }
}
