using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.VertrouwelijkheidMapping.ShowVertrouwelijkheidMappings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.VertrouwelijkheidMapping.ShowVertrouwelijkheidMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/vertrouwelijkheid")]
public class ShowVertrouwelijkheidMappingsController(DatamigratieDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<VertrouwelijkheidMappingResponse>>> GetVertrouwelijkheidMappings(
        [FromRoute] Guid zaaktypenMappingId)
    {
        var mappings = await context.VertrouwelijkheidMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(m => new VertrouwelijkheidMappingResponse
            {
                DetVertrouwelijkheid = m.DetVertrouwelijkheid,
                OzVertrouwelijkheidaanduiding = m.OzVertrouwelijkheidaanduiding
            })
            .ToListAsync();

        return Ok(mappings);
    }
}
