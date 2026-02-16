using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.ShowDocumentPropertyMappings.Models;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.ShowDocumentPropertyMappings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.ShowDocumentPropertyMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/documentproperties")]
public class ShowDocumentPropertyMappingsController(IShowDocumentPropertyMappingsService showService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<DocumentPropertyMappingResponse>>> GetDocumentPropertyMappings(
        [FromRoute] Guid zaaktypenMappingId)
    {
        var mappings = await showService.GetDocumentPropertyMappings(zaaktypenMappingId);
        return Ok(mappings);
    }
}
