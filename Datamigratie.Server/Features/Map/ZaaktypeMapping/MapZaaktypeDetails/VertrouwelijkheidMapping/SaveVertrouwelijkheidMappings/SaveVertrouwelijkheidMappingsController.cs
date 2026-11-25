using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.VertrouwelijkheidMapping.SaveVertrouwelijkheidMappings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.VertrouwelijkheidMapping.SaveVertrouwelijkheidMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/vertrouwelijkheid")]
public class SaveVertrouwelijkheidMappingsController(DatamigratieDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveVertrouwelijkheidMappings(
        [FromRoute] Guid zaaktypenMappingId,
        [FromBody] SaveVertrouwelijkheidMappingsRequest request)
    {
        var existingMappings = await context.VertrouwelijkheidMappings
            .Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();

        context.VertrouwelijkheidMappings.RemoveRange(existingMappings);

        // TODO DATA-247: take the zaaktype version from the route once mapping per version is in place
        var zaaktypeVersieMappingId = await context.ZaaktypeVersieMappings
            .Where(v => v.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(v => v.Id)
            .SingleAsync();

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
