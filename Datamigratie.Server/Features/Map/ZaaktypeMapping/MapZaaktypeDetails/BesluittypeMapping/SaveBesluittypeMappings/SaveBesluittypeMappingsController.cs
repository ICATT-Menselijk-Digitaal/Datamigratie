using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Models;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/besluittypen")]
public class SaveBesluittypeMappingsController(ISaveBesluittypeMappingsService saveBesluittypeMappingsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveBesluittypeMappings(
        [FromRoute] Guid zaaktypenMappingId,
        [FromBody] SaveBesluittypeMappingsRequest request)
    {
        await saveBesluittypeMappingsService.SaveBesluittypeMappings(zaaktypenMappingId, request);
        return Ok();
    }
}
