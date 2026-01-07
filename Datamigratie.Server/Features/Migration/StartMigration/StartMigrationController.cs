using Datamigratie.Common.Services.Det;
using Datamigratie.Data;
using Datamigratie.Server.Features.Migration.StartMigration.Models;
using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migration.StartMigration.Services;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Datamigratie.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.StartMigration;

[ApiController]
[Route("api/migration")]
public class StartMigrationController(
    MigrationWorkerState workerState, 
    IMigrationBackgroundTaskQueue backgroundTaskQueue,
    DatamigratieDbContext dbContext,
    ILogger<StartMigrationController> logger) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult> StartMigration([FromBody] StartMigrationRequest request)
    {
        // perform some validation so the frontend gets quick feedback if something is wrong
        if (workerState.IsWorking)
        {
            return Conflict(new { message = "Er loopt al een migratie." });
        }

        try
        {
            var globalMapping = await GetAndValidateGlobalMappingAsync();
            
            await backgroundTaskQueue.QueueMigrationAsync(new MigrationQueueItem 
            { 
                DetZaaktypeId = request.DetZaaktypeId,
                GlobalMapping = globalMapping
            });
            
            return Ok();
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new { message = "Migratie kan niet starten: Er is geen globale configuratie gevonden. Configureer een geldig RSIN in de globale configuratie pagina." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = $"Migratie kan niet starten: Ongeldig RSIN - {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<MigrationStatusResponse>> GetMigration()
    {
        if (!workerState.IsWorking)
        {
            return Ok(new MigrationStatusResponse() { Status = ServiceMigrationStatus.None });
        }

        if (workerState.DetZaaktypeId == null)
        {
            throw new InvalidDataException("Worker is running a migration without a DetZaaktypeId.");
        }

        return new MigrationStatusResponse()
        {
            Status = ServiceMigrationStatus.InProgress,
            DetZaaktypeId = workerState.DetZaaktypeId,
        };
    }

    private async Task<GlobalMapping> GetAndValidateGlobalMappingAsync()
    {
        var globalMapping = await dbContext.GlobalConfigurations
            .Select(x => new GlobalMapping { Rsin = x.Rsin! })
            .SingleAsync();

        RsinValidator.ValidateRsin(globalMapping.Rsin, logger);

        return globalMapping;
    }
}
