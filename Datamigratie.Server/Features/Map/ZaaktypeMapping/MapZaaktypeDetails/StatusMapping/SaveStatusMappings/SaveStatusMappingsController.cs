using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.SaveStatusMappings.Models;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.SaveStatusMappings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.SaveStatusMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/statuses")]
public class SaveStatusMappingsController(ISaveStatusMappingsService saveStatusMappingsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveStatusMappings(
        [FromRoute] Guid zaaktypenMappingId,
        [FromBody] SaveStatusMappingsRequest request)
    {
        await saveStatusMappingsService.SaveStatusMappings(zaaktypenMappingId, request);
        return Ok();
    }
}
