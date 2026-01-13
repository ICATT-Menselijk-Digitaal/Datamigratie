using Datamigratie.Server.Features.Migration.StartMigration.Models;
using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Datamigratie.Server.Helpers;
using Datamigratie.Server.Features.Mapping.StatusMapping.ValidateStatusMappings.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Datamigratie.Data;
using Datamigratie.Common.Services.Det;

namespace Datamigratie.Server.Features.Migration.StartMigration;

[ApiController]
[Route("api/migration")]
public class StartMigrationController(
    MigrationWorkerState workerState, 
    IMigrationBackgroundTaskQueue backgroundTaskQueue,
    DatamigratieDbContext dbContext,
    IValidateStatusMappingsService validateStatusMappingsService,
    ILogger<StartMigrationController> logger,
    IDetApiClient detApiClient) : ControllerBase
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
            // validating all statuses are mapped before starting migration
            var allStatusesMapped = await validateStatusMappingsService.AreAllStatusesMapped(request.DetZaaktypeId);
            if (!allStatusesMapped)
            {
                return BadRequest(new { message = "Not all DET statuses have been mapped to OZ statuses. Please configure status mappings first." });
            }
            
            var globalMapping = await GetAndValidateGlobalMappingAsync();

            await backgroundTaskQueue.QueueMigrationAsync(new MigrationQueueItem
            {
                DetZaaktypeId = request.DetZaaktypeId,
                GlobalMapping = globalMapping
            });

            // Validate that all DET resultaattypen have been mapped
            var detZaaktype = await detApiClient.GetZaaktype(request.DetZaaktypeId);
            if (detZaaktype == null)
            {
                return NotFound(new { message = $"DET Zaaktype with id {request.DetZaaktypeId} not found." });
            }

            if (detZaaktype.Resultaten != null && detZaaktype.Resultaten.Count > 0)
            {
                var existingMappings = await dbContext.ResultaattypeMappings
                    .Include(m => m.ZaaktypenMapping)
                    .Where(m => m.ZaaktypenMapping.DetZaaktypeId == request.DetZaaktypeId)
                    .Select(m => m.DetResultaattypeId)
                    .ToListAsync();

                var unmappedResultaten = detZaaktype.Resultaten
                    .Where(r => !existingMappings.Contains(r.Resultaat.Naam))
                    .Select(r => r.Resultaat.Naam)
                    .ToList();

                if (unmappedResultaten.Count > 0)
                {
                    return Conflict(new
                    {
                        message = "Not all DET resultaattypen have been mapped to OZ resultaattypen.",
                        unmappedResultaten
                    });
                }
            }
        }
        catch (Exception e)
        {
            return Conflict(new { message = e.Message });
        }
        return Ok();
        
    }

    [HttpGet]
    public Task<ActionResult<MigrationStatusResponse>> GetMigration()
    {
        if (!workerState.IsWorking)
        {
            return Task.FromResult<ActionResult<MigrationStatusResponse>>(Ok(new MigrationStatusResponse() { Status = ServiceMigrationStatus.None }));
        }

        if (workerState.DetZaaktypeId == null)
        {
            throw new InvalidDataException("Worker is running a migration without a DetZaaktypeId.");
        }

        return Task.FromResult<ActionResult<MigrationStatusResponse>>(new MigrationStatusResponse()
        {
            Status = ServiceMigrationStatus.InProgress,
            DetZaaktypeId = workerState.DetZaaktypeId,
        });
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
