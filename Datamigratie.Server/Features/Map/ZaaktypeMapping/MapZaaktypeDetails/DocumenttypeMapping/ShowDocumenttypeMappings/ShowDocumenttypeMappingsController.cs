using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.ShowDocumenttypeMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.ShowDocumenttypeMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/documenttypen")]
public class ShowDocumenttypeMappingsController(IShowDocumenttypeMappingsService showDocumenttypeMappingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<DocumenttypeMappingResponse>>> GetDocumenttypeMappings([FromRoute] Guid zaaktypenMappingId)
    {
        var mappings = await showDocumenttypeMappingsService.GetDocumenttypeMappings(zaaktypenMappingId);
        return Ok(mappings);
    }
}
