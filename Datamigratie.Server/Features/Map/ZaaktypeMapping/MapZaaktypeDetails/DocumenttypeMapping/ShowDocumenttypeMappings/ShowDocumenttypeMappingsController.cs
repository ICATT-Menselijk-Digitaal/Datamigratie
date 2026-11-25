using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.ShowDocumenttypeMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.ShowDocumenttypeMappings;

[ApiController]
[Route("api/mappings/versies/{zaaktypeVersieMappingId:guid}/documenttypen")]
public class ShowDocumenttypeMappingsController(IShowDocumenttypeMappingsService showDocumenttypeMappingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<DocumenttypeMappingResponse>>> GetDocumenttypeMappings([FromRoute] Guid zaaktypeVersieMappingId)
    {
        var mappings = await showDocumenttypeMappingsService.GetDocumenttypeMappings(zaaktypeVersieMappingId);
        return Ok(mappings);
    }
}
