using Datamigratie.Server.Features.Mapping.StatusMapping.SaveStatusMappings.Models;
using Datamigratie.Server.Features.Mapping.StatusMapping.SaveStatusMappings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.StatusMapping.SaveStatusMappings;

[ApiController]
[Route("api/mappings/{detZaaktypeId}/statuses")]
public class SaveStatusMappingsController(ISaveStatusMappingsService saveStatusMappingsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveStatusMappings(
        [FromRoute] string detZaaktypeId,
        [FromBody] SaveStatusMappingsRequest request)
    {
        if (request.DetZaaktypeId != detZaaktypeId)
        {
            return BadRequest(new { message = "DetZaaktypeId in route does not match request body" });
        }

        await saveStatusMappingsService.SaveStatusMappings(request);
        return Ok();
    }
}
