using Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Models;
using Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings;

[ApiController]
[Route("api/mappings/{detZaaktypeId}/statuses")]
public class ShowStatusMappingsController(IShowStatusMappingsService showStatusMappingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<StatusMappingsResponse>> GetStatusMappings(
        [FromRoute] string detZaaktypeId)
    {
        var result = await showStatusMappingsService.GetStatusMappings(detZaaktypeId);
        return Ok(result);
    }
}
