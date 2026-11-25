using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PdfInformatieobjecttypeMapping.SavePdfInformatieobjecttypeMapping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PdfInformatieobjecttypeMapping.SavePdfInformatieobjecttypeMapping;

[ApiController]
[Route("api/mappings/versies/{zaaktypeVersieMappingId:guid}/informatieobjecttype")]
public class SavePdfInformatieobjecttypeMappingController(DatamigratieDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SavePdfInformatieobjecttypeMapping(
        [FromRoute] Guid zaaktypeVersieMappingId,
        [FromBody] SavePdfInformatieobjecttypeMappingRequest request)
    {
        var existingMapping = await context.PdfInformatieobjecttypeMappings
            .Where(m => m.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
            .FirstOrDefaultAsync();

        if (existingMapping is not null)
            context.PdfInformatieobjecttypeMappings.Remove(existingMapping);

        if (request.OzInformatieobjecttypeId is not null)
        {
            await context.PdfInformatieobjecttypeMappings.AddAsync(new Data.Entities.PdfInformatieobjecttypeMapping
            {
                ZaaktypeVersieMappingId = zaaktypeVersieMappingId,
                OzInformatieobjecttypeId = request.OzInformatieobjecttypeId.Value
            });
        }

        await context.SaveChangesAsync();

        return Ok();
    }
}
