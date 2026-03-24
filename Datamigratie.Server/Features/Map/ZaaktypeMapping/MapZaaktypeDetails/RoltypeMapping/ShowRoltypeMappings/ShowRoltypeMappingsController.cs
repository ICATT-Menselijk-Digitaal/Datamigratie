using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/roltypen")]
public class ShowRoltypeMappingsController(IShowRoltypeMappingsService showRoltypeMappingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoltypeMappingResponse>>> GetRoltypeMappings([FromRoute] Guid zaaktypenMappingId)
    {
        var mappings = await showRoltypeMappingsService.GetRoltypeMappings(zaaktypenMappingId);
        return Ok(mappings);
    }
}
