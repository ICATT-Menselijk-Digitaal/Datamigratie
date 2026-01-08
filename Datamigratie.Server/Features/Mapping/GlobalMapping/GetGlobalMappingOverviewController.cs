using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.GlobalMapping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.GlobalMapping;

[ApiController]
[Route("api/globalmapping")]
public class GetGlobalMappingOverviewController(
    DatamigratieDbContext dbContext,
    ILogger<GetGlobalMappingOverviewController> logger) : ControllerBase
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
}
