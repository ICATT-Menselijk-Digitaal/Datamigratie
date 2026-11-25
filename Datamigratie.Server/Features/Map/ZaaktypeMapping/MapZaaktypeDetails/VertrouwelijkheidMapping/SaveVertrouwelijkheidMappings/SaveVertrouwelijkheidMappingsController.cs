using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.VertrouwelijkheidMapping.SaveVertrouwelijkheidMappings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.VertrouwelijkheidMapping.SaveVertrouwelijkheidMappings;

[ApiController]
[Route("api/mappings/versies/{zaaktypeVersieMappingId:guid}/vertrouwelijkheid")]
public class SaveVertrouwelijkheidMappingsController(DatamigratieDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveVertrouwelijkheidMappings(
        [FromRoute] Guid zaaktypeVersieMappingId,
        [FromBody] SaveVertrouwelijkheidMappingsRequest request)
    {
        var existingMappings = await context.VertrouwelijkheidMappings
            .Where(m => m.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
            .ToListAsync();

        context.VertrouwelijkheidMappings.RemoveRange(existingMappings);

        var newMappings = request.Mappings.Select(m => new Data.Entities.VertrouwelijkheidMapping
        {
            ZaaktypeVersieMappingId = zaaktypeVersieMappingId,
            DetVertrouwelijkheid = m.DetVertrouwelijkheid,
            OzVertrouwelijkheidaanduiding = m.OzVertrouwelijkheidaanduiding.ToString()
        });

        await context.VertrouwelijkheidMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();

        return Ok();
    }
}
