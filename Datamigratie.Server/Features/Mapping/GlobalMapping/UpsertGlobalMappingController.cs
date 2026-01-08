using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.GlobalMapping.Models;
using Datamigratie.Server.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.GlobalMapping;

[ApiController]
[Route("api/globalmapping")]
public class UpsertGlobalMappingController(
    DatamigratieDbContext dbContext,
    ILogger<UpsertGlobalMappingController> logger) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<GlobalConfigurationResponseModel>> UpsertConfiguration(
        [FromBody] GlobalConfigurationRequestModel request)
    {
        try
        {
            // Validate RSIN if provided
            if (!string.IsNullOrWhiteSpace(request.Rsin))
            {
                RsinValidator.ValidateRsin(request.Rsin, logger);
            }

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
