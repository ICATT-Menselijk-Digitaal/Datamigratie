using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.SaveDocumentPropertyMappings.Models;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.SaveDocumentPropertyMappings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.SaveDocumentPropertyMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/documentproperties")]
public class SaveDocumentPropertyMappingsController(ISaveDocumentPropertyMappingsService saveService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveDocumentPropertyMappings(
        [FromRoute] Guid zaaktypenMappingId,
        [FromBody] SaveDocumentPropertyMappingsRequest request)
    {
        await saveService.SaveDocumentPropertyMappings(zaaktypenMappingId, request);
        return Ok();
    }
}
