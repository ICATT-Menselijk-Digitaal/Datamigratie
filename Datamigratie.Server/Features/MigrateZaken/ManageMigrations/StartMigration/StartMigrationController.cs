using Datamigratie.Common.Services.Det;
using Datamigratie.Data;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.Resultaattypen.ValidateMappings.Services;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.StatusMapping.ValidateMappings.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;
using Datamigratie.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration;

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

            var globalMapping = await ValidateAndGetGlobalMappingAsync();

            var statusMappings = await ValidateAndGetStatusMappingsAsync(detZaaktype);
            var resultaatMappings = await ValidateAndGetResultaattypeMappingsAsync(detZaaktype);

            await backgroundTaskQueue.QueueMigrationAsync(new MigrationQueueItem
            {
                DetZaaktypeId = request.DetZaaktypeId,
                GlobalMapping = globalMapping,
                StatusMappings = statusMappings,
                ResultaatMappings = resultaatMappings
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

    private async Task<GlobalMapping> ValidateAndGetGlobalMappingAsync()
    {
        var globalMapping = await dbContext.GlobalConfigurations
            .Select(x => new GlobalMapping { Rsin = x.Rsin! })
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException("Geen globale configuratie gevonden.");

        RsinValidator.ValidateRsin(globalMapping.Rsin, logger);

        return globalMapping;
    }

    private async Task<Dictionary<string, Guid>> ValidateAndGetStatusMappingsAsync(Common.Services.Det.Models.DetZaaktypeDetail detZaaktype)
    {
        var (statusMappingsValid, statusMappings) = await validateStatusMappingsService.ValidateAndGetStatusMappings(detZaaktype);

        return !statusMappingsValid
            ? throw new InvalidOperationException("Not all DET statuses have been mapped to OZ statuses. Please configure status mappings first.")
            : statusMappings;
    }

    private async Task<Dictionary<string, Guid>> ValidateAndGetResultaattypeMappingsAsync(Common.Services.Det.Models.DetZaaktypeDetail detZaaktype)
    {
        var (resultaatMappingsValid, resultaatMappings) = await validateResultaattypeMappingsService.ValidateAndGetResultaattypeMappings(detZaaktype);

        return !resultaatMappingsValid
            ? throw new InvalidOperationException("Not all DET Resultaattypen have been mapped to OZ resultaattypen. Please configure resultaattypen mappings first.")
            : resultaatMappings;
    }
}
