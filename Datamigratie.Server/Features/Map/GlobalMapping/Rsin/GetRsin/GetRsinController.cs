using Datamigratie.Data;
using Datamigratie.Server.Features.Map.GlobalMapping.Rsin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.GlobalMapping.Rsin;

[ApiController]
[Route("api/globalmapping/rsin")]
public class GetRsinController(
    DatamigratieDbContext dbContext,
    ILogger<GetRsinController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GetRsinResponseModel>> GetConfiguration()
    {
        try
        {
            var config = await dbContext.RsinConfiguration.FirstOrDefaultAsync();

            return config == null
                ? Ok(new GetRsinResponseModel())
                : Ok(new GetRsinResponseModel
                {
                    Rsin = config.Rsin,
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting rsin configuration");
            return StatusCode(500, "Er is een fout opgetreden bij het ophalen van de rsin.");
        }
    }
}
