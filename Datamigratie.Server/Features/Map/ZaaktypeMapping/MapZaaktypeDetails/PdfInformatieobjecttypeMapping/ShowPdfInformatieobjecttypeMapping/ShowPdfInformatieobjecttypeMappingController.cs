using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PdfInformatieobjecttypeMapping.ShowPdfInformatieobjecttypeMapping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PdfInformatieobjecttypeMapping.ShowPdfInformatieobjecttypeMapping;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/informatieobjecttype")]
public class ShowPdfInformatieobjecttypeMappingController(DatamigratieDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PdfInformatieobjecttypeMappingResponse?>> GetPdfInformatieobjecttypeMapping(
        [FromRoute] Guid zaaktypenMappingId)
    {
        var mapping = await context.PdfInformatieobjecttypeMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMappingId)
            .FirstOrDefaultAsync();

        if (mapping is null)
            return Ok(null);

        return Ok(new PdfInformatieobjecttypeMappingResponse
        {
            OzInformatieobjecttypeId = mapping.OzInformatieobjecttypeId
        });
    }
}
