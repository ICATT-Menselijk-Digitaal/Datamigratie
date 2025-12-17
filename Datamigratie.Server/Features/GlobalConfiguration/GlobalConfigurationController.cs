using Datamigratie.Server.Features.GlobalConfiguration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.GlobalConfiguration;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GlobalConfigurationController(
    IGlobalConfigurationService globalConfigurationService,
    ILogger<GlobalConfigurationController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GlobalConfigurationResponse>> GetConfiguration()
    {
        try
        {
            var config = await globalConfigurationService.GetConfigurationAsync();
            return Ok(config);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting global configuration");
            return StatusCode(500, "Er is een fout opgetreden bij het ophalen van de configuratie.");
        }
    }

    [HttpPut]
    public async Task<ActionResult<GlobalConfigurationResponse>> UpdateConfiguration(
        [FromBody] UpdateGlobalConfigurationRequest request)
    {
        try
        {
            var config = await globalConfigurationService.UpdateConfigurationAsync(request);
            return Ok(config);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid configuration update request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating global configuration");
            return StatusCode(500, "Er is een fout opgetreden bij het bijwerken van de configuratie.");
        }
    }
}
