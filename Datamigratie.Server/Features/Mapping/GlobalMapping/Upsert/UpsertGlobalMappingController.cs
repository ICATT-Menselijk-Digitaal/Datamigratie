using Datamigratie.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.GlobalMapping.Upsert;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class UpsertGlobalMappingController(
    DatamigratieDbContext dbContext,
    ILogger<UpsertGlobalMappingController> logger) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<GlobalConfigurationResponseModel>> UpdateConfiguration([FromBody] UpsertRequestModel request)
    {
        try
        {
            // Validate RSIN if provided
            if (!string.IsNullOrWhiteSpace(request.Rsin))
            {
                if (!RsinValidator.IsValid(request.Rsin))
                {
                    var error = RsinValidator.GetValidationError(request.Rsin);
                    logger.LogWarning("Invalid RSIN provided: {Rsin}. Error: {Error}", request.Rsin, error);
                    throw new InvalidOperationException(error); 
                }
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
