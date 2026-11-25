using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.ShowPublicatieNiveauMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.ShowPublicatieNiveauMappings;

[ApiController]
[Route("api/mappings/versies/{zaaktypeVersieMappingId:guid}/publicatieniveaus")]
public class ShowPublicatieNiveauMappingsController(IShowPublicatieNiveauMappingsService showPublicatieNiveauMappingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PublicatieNiveauMappingResponse>>> GetPublicatieNiveauMappings([FromRoute] Guid zaaktypeVersieMappingId)
    {
        var mappings = await showPublicatieNiveauMappingsService.GetPublicatieNiveauMappings(zaaktypeVersieMappingId);
        return Ok(mappings);
    }
}
