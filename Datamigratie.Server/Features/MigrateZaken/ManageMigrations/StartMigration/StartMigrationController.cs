using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Data;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documentstatus;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Resultaat;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Status;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.DocumentProperty;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Vertrouwelijkheid;
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
    IValidateDocumentstatusMappingsService validateDocumentstatusMappingsService,
    IValidateDocumentPropertyMappingsService validateDocumentPropertyMappingsService,
    IValidateVertrouwelijkheidMappingsService validateVertrouwelijkheidMappingsService,
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

            var rsinMapping = await ValidateAndGetRsinMappingAsync();

            var statusMappings = await ValidateAndGetStatusMappingsAsync(detZaaktype);
            var resultaatMappings = await ValidateAndGetResultaattypeMappingsAsync(detZaaktype);
            var documentstatusMappings = await ValidateAndGetDocumentstatusMappingsAsync();
            var documentPropertyMappings = await ValidateAndGetDocumentPropertyMappingsAsync(detZaaktype);
            var vertrouwelijkheidMappings = await ValidateAndGetVertrouwelijkheidMappingsAsync(detZaaktype);

            await backgroundTaskQueue.QueueMigrationAsync(new MigrationQueueItem
            {
                DetZaaktypeId = request.DetZaaktypeId,
                RsinMapping = rsinMapping,
                StatusMappings = statusMappings,
                ResultaatMappings = resultaatMappings,
                DocumentstatusMappings = documentstatusMappings,
                DocumentPropertyMappings = documentPropertyMappings,
                VertrouwelijkheidMappings = vertrouwelijkheidMappings
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

    private async Task<RsinMapping> ValidateAndGetRsinMappingAsync()
    {
        var rsinMapping = await dbContext.RsinConfigurations
            .Select(x => new RsinMapping { Rsin = x.Rsin! })
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException("Geen rsin configuratie gevonden.");

        RsinValidator.ValidateRsin(rsinMapping.Rsin, logger);

        return rsinMapping;
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

    private async Task<Dictionary<string, string>> ValidateAndGetDocumentstatusMappingsAsync()
    {
        var (documentstatusMappingsValid, documentstatusMappings) = await validateDocumentstatusMappingsService.ValidateAndGetDocumentstatusMappings();

        return !documentstatusMappingsValid
            ? throw new InvalidOperationException("Not all DET document statuses have been mapped to OZ document statuses. Please configure document status mappings first.")
            : documentstatusMappings;
    }

    private async Task<Dictionary<string, Dictionary<string, string>>> ValidateAndGetDocumentPropertyMappingsAsync(Common.Services.Det.Models.DetZaaktypeDetail detZaaktype)
    {
        var (documentPropertyMappingsValid, documentPropertyMappings) = await validateDocumentPropertyMappingsService.ValidateAndGetDocumentPropertyMappings(detZaaktype);

        return !documentPropertyMappingsValid
            ? throw new InvalidOperationException("Not all document properties have been mapped. Please configure publicatieniveau and documenttype mappings first.")
            : documentPropertyMappings;
    }

    private async Task<Dictionary<bool, VertrouwelijkheidsAanduiding>> ValidateAndGetVertrouwelijkheidMappingsAsync(Common.Services.Det.Models.DetZaaktypeDetail detZaaktype)
    {
        var (vertrouwelijkheidMappingsValid, vertrouwelijkheidMappings) = await validateVertrouwelijkheidMappingsService.ValidateAndGetVertrouwelijkheidMappings(detZaaktype);

        return !vertrouwelijkheidMappingsValid
            ? throw new InvalidOperationException("Not all vertrouwelijkheid values have been mapped. Please configure vertrouwelijkheid mappings first.")
            : vertrouwelijkheidMappings;
    }
}
