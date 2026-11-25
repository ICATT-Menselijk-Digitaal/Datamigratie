using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PdfInformatieobjecttypeMapping.SavePdfInformatieobjecttypeMapping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PdfInformatieobjecttypeMapping.SavePdfInformatieobjecttypeMapping;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/informatieobjecttype")]
public class SavePdfInformatieobjecttypeMappingController(DatamigratieDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SavePdfInformatieobjecttypeMapping(
        [FromRoute] Guid zaaktypenMappingId,
        [FromBody] SavePdfInformatieobjecttypeMappingRequest request)
    {
        var existingMapping = await context.PdfInformatieobjecttypeMappings
            .Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId)
            .FirstOrDefaultAsync();

        if (existingMapping is not null)
            context.PdfInformatieobjecttypeMappings.Remove(existingMapping);

        // TODO DATA-247: take the zaaktype version from the route once mapping per version is in place
        var zaaktypeVersieMappingId = await context.ZaaktypeVersieMappings
            .Where(v => v.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(v => v.Id)
            .SingleAsync();

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
