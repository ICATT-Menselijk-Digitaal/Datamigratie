using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Models;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings;

[ApiController]
[Route("api/mappings/versies/{zaaktypeVersieMappingId:guid}/besluittypen")]
public class SaveBesluittypeMappingsController(ISaveBesluittypeMappingsService saveBesluittypeMappingsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveBesluittypeMappings(
        [FromRoute] Guid zaaktypeVersieMappingId,
        [FromBody] SaveBesluittypeMappingsRequest request)
    {
        await saveBesluittypeMappingsService.SaveBesluittypeMappings(zaaktypeVersieMappingId, request);
        return Ok();
    }
}
