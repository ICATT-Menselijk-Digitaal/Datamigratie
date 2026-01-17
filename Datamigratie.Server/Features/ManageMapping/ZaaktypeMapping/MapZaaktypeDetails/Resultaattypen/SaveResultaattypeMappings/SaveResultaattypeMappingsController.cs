using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.Resultaattypen.SaveResultaattypeMappings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.Resultaattypen.SaveResultaattypeMappings
{
    [ApiController]
    [Route("api/mappings/{zaaktypenMappingId:guid}/resultaattypen")]
    public class SaveResultaattypenMappingsController(ISaveResultaattypenMappingsService saveResultaattypenMappingsService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> SaveStatusMappings(
            [FromRoute] Guid zaaktypenMappingId,
            [FromBody] SaveResultaattypeMappingRequest request)
        {
            await saveResultaattypenMappingsService.SaveResultaattypeMappings(zaaktypenMappingId, request);
            return Ok();
        }
    }
}
