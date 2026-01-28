using Datamigratie.Data;
using Datamigratie.Server.Features.Map.GlobalMapping.Rsin.Models;
using Datamigratie.Server.Features.Map.GlobalMapping.Rsin.Save.Models;
using Datamigratie.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.GlobalMapping.Rsin.Save;

[ApiController]
[Route("api/globalmapping/rsin")]
public class SaveRsinController(
    DatamigratieDbContext dbContext,
    ILogger<SaveRsinController> logger) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<GetRsinResponseModel>> UpdateRsin(
        [FromBody] SaveRsinRequestModel request)
    {
        try
        {
            // Validate RSIN if provided
            if (!string.IsNullOrWhiteSpace(request.Rsin))
            {
                RsinValidator.ValidateRsin(request.Rsin, logger);
            }

            var config = await dbContext.RsinConfiguration.FirstOrDefaultAsync();

            if (config == null)
            {
                // Create new configuration
                config = new Data.Entities.RsinConfiguration();
                dbContext.RsinConfiguration.Add(config);
            }

            config.Rsin = request.Rsin;

            await dbContext.SaveChangesAsync();

            return new GetRsinResponseModel
            {
                Rsin = config.Rsin,
            };
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating rsin configuration");
            return StatusCode(500, "Er is een fout opgetreden bij het bijwerken van de rsin configuratie.");
        }
    }
}
