using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.ShowPublicatieNiveauMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.ShowPublicatieNiveauMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/publicatieniveaus")]
public class ShowPublicatieNiveauMappingsController(IShowPublicatieNiveauMappingsService showPublicatieNiveauMappingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PublicatieNiveauMappingResponse>>> GetPublicatieNiveauMappings([FromRoute] Guid zaaktypenMappingId)
    {
        var mappings = await showPublicatieNiveauMappingsService.GetPublicatieNiveauMappings(zaaktypenMappingId);
        return Ok(mappings);
    }
}
