using Datamigratie.Server.Features.Mapping.StatusMapping.Models;
using Datamigratie.Server.Features.Mapping.StatusMapping.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.StatusMapping;

[ApiController]
[Route("api/mappings/{detZaaktypeId}/statuses")]
public class StatusMappingController(IStatusMappingService statusMappingService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<StatusMappingsResponse>> GetStatusMappings(
        [FromRoute] string detZaaktypeId)
    {
        var result = await statusMappingService.GetStatusMappings(detZaaktypeId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SaveStatusMappings(
        [FromRoute] string detZaaktypeId,
        [FromBody] SaveStatusMappingsRequest request)
    {
        Console.WriteLine($"SaveStatusMappings called - Route detZaaktypeId: {detZaaktypeId}, Request detZaaktypeId: {request.DetZaaktypeId}, Mappings count: {request.Mappings.Count}");
        
        if (request.DetZaaktypeId != detZaaktypeId)
        {
            return BadRequest(new { message = "DetZaaktypeId in route does not match request body" });
        }

        await statusMappingService.SaveStatusMappings(request);
        return Ok();
    }
}
