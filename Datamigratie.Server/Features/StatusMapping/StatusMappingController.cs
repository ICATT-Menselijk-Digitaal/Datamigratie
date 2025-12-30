using Datamigratie.Server.Features.StatusMapping.Models;
using Datamigratie.Server.Features.StatusMapping.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.StatusMapping;

[ApiController]
[Route("api/status-mappings")]
public class StatusMappingController(IStatusMappingService statusMappingService) : ControllerBase
{
    [HttpGet("{detZaaktypeId}")]
    public async Task<ActionResult<StatusMappingsResponse>> GetStatusMappings(string detZaaktypeId)
    {
        try
        {
            var result = await statusMappingService.GetStatusMappingsForZaaktype(detZaaktypeId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> SaveStatusMappings([FromBody] SaveStatusMappingsRequest request)
    {
        try
        {
            await statusMappingService.SaveStatusMappings(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{detZaaktypeId}/validation")]
    public async Task<ActionResult<bool>> ValidateStatusMappings(string detZaaktypeId)
    {
        var allStatusesMapped = await statusMappingService.AreAllStatusesMapped(detZaaktypeId);
        return Ok(new { allStatusesMapped });
    }
}
