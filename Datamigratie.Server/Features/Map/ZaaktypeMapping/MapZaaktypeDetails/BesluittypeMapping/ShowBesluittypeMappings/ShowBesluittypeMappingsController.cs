using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.ShowBesluittypeMappings.Models;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.ShowBesluittypeMappings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.ShowBesluittypeMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/besluittypen")]
public class ShowBesluittypeMappingsController(IShowBesluittypeMappingsService showBesluittypeMappingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BesluittypeMappingsResponse>>> GetBesluittypeMappings(
        [FromRoute] Guid zaaktypenMappingId)
    {
        var result = await showBesluittypeMappingsService.GetBesluittypeMappings(zaaktypenMappingId);
        return Ok(result);
    }
}
