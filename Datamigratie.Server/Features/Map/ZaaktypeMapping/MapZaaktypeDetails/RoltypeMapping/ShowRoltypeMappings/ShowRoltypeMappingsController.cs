using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings;

[ApiController]
[Route("api/mappings/versies/{zaaktypeVersieMappingId:guid}/roltypen")]
public class ShowRoltypeMappingsController(IShowRoltypeMappingsService showRoltypeMappingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoltypeMappingResponse>>> GetRoltypeMappings([FromRoute] Guid zaaktypeVersieMappingId)
    {
        var mappings = await showRoltypeMappingsService.GetRoltypeMappings(zaaktypeVersieMappingId);
        return Ok(mappings);
    }
}
