using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Server.Features.Mapping.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.MapResultaattypen
{
    [ApiController]
    [Route("api/mapping/resultaattype/")]
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
