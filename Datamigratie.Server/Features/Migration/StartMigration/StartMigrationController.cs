using Datamigratie.Server.Features.Migration.StartMigration.Models;
using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Datamigratie.Server.Helpers;
using Datamigratie.Server.Features.Mapping.StatusMapping.ValidateMappings.Services;
using Datamigratie.Server.Features.Mapping.Resultaattypen.ValidateMappings.Services;
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
    IValidateResultaattypeMappingsService validateResultaattypeMappingsService,
    IDetApiClient detApiClient,
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
            // Fetch zaaktype details once to avoid duplicate API calls
            var detZaaktype = await detApiClient.GetZaaktypeDetail(request.DetZaaktypeId);
            if (detZaaktype == null)
            {
                return BadRequest(new { message = "DET Zaaktype not found." });
            }

            // validating all statuses are mapped before starting migration
            var allStatusesMapped = await validateStatusMappingsService.AreAllStatusesMapped(detZaaktype);
            if (!allStatusesMapped)
            {
                return BadRequest(new { message = "Not all DET statuses have been mapped to OZ statuses. Please configure status mappings first." });
            }

            var allResultaattypenMapped = await validateResultaattypeMappingsService.AreAllResultaattypenMapped(detZaaktype);
            if (!allResultaattypenMapped)
            {
                return BadRequest(new { message = "Not all DET Resultaattypen have been mapped to OZ resultaattypen. Please configure resultaattypen mappings first." });
            }

            var globalMapping = await GetAndValidateGlobalMappingAsync();

            await backgroundTaskQueue.QueueMigrationAsync(new MigrationQueueItem
            {
                DetZaaktypeId = request.DetZaaktypeId,
                GlobalMapping = globalMapping
            });
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
