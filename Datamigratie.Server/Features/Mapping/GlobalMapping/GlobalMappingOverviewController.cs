using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.GlobalMapping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.GlobalMapping;

[ApiController]
[Route("api/globalmapping")]
//[Authorize]
public class GlobalMappingOverviewController(
    DatamigratieDbContext dbContext,
    ILogger<GlobalMappingOverviewController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GlobalConfigurationResponseModel>> GetConfiguration()
    {
        try
        {
            var config = await dbContext.GlobalConfigurations.FirstOrDefaultAsync();

            return config == null
                ? Ok(new GlobalConfigurationResponseModel())
                : Ok(new GlobalConfigurationResponseModel
                {
                    Rsin = config.Rsin,
                    UpdatedAt = config.UpdatedAt
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting global configuration");
            return StatusCode(500, "Er is een fout opgetreden bij het ophalen van de configuratie.");
        }
    }

    [HttpPut]
    public async Task<ActionResult<GlobalConfigurationResponseModel>> UpdateConfiguration([FromBody] GlobalConfigurationRequestModel request)
    {
        try
        {
            // Validate RSIN if provided
            RsinValidator.ValidateRsin(request.Rsin, logger);

            var config = await dbContext.GlobalConfigurations.FirstOrDefaultAsync();

            if (config == null)
            {
                // Create new configuration
                config = new Data.Entities.GlobalConfiguration();
                dbContext.GlobalConfigurations.Add(config);
            }

            config.Rsin = request.Rsin;
            config.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return new GlobalConfigurationResponseModel
            {
                Rsin = config.Rsin,
                UpdatedAt = config.UpdatedAt
            };
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating global configuration");
            return StatusCode(500, "Er is een fout opgetreden bij het bijwerken van de configuratie.");
        }
    }
}
