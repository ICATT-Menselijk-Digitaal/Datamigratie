using Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Models;
using Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings;

[ApiController]
[Route("api/mappings/{zaaktypenMappingId:guid}/statuses")]
public class ShowStatusMappingsController(IShowStatusMappingsService showStatusMappingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<StatusMappingsResponse>> GetStatusMappings(
        [FromRoute] Guid zaaktypenMappingId)
    {
        var result = await showStatusMappingsService.GetStatusMappings(zaaktypenMappingId);
        return Ok(result);
    }
}
