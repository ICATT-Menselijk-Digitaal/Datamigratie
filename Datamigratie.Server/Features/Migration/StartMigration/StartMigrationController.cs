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
        }

        // Validate that all DET resultaattypen have been mapped
        var detZaaktype = await detApiClient.GetZaaktype(request.DetZaaktypeId);
        if (detZaaktype == null)
        {
            return NotFound(new { message = $"DET Zaaktype with id {request.DetZaaktypeId} not found." });
        }

        if (detZaaktype.Resultaten != null && detZaaktype.Resultaten.Count > 0)
        {
            var existingMappings = await context.ResultaattypeMappings
                .Where(m => m.DetZaaktypeId == request.DetZaaktypeId)
                .Select(m => m.DetResultaattypeId)
                .ToListAsync();

            var unmappedResultaten = detZaaktype.Resultaten
                .Where(r => !existingMappings.Contains(r.Resultaat.Naam))
                .Select(r => r.Resultaat.Naam)
                .ToList();

            if (unmappedResultaten.Count > 0)
            {
                return BadRequest(new
                {
                    message = "Not all DET resultaattypen have been mapped to OZ resultaattypen.",
                    unmappedResultaten = unmappedResultaten
                });
            }
        }

        catch (Exception e)
        {
            return Conflict(new { message = e.Message });
        }
        return Ok();
        
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
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException("Geen globale configuratie gevonden.");

        RsinValidator.ValidateRsin(globalMapping.Rsin, logger);

        return globalMapping;
    }
}
