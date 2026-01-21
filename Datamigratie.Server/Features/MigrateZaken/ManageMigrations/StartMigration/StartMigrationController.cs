using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Data;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Resultaat;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Status;
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

            var rsinMapping = await ValidateAndGetRsinMappingAsync();

            var statusMappings = await ValidateAndGetStatusMappingsAsync(detZaaktype);
            var resultaatMappings = await ValidateAndGetResultaattypeMappingsAsync(detZaaktype);
            var documentstatusMappings = await ValidateAndGetDocumentstatusMappingsAsync();

            await backgroundTaskQueue.QueueMigrationAsync(new MigrationQueueItem
            {
                DetZaaktypeId = request.DetZaaktypeId,
                RsinMapping = rsinMapping,
                StatusMappings = statusMappings,
                ResultaatMappings = resultaatMappings,
                DocumentstatusMappings = documentstatusMappings
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
        // Get all document status mappings from database
        var documentstatusMappings = await dbContext.DocumentstatusMappings
            .ToDictionaryAsync(m => m.DetDocumentstatus, m => m.OzDocumentstatus);

        // Check if any mappings exist
        if (documentstatusMappings.Count == 0)
        {
            throw new InvalidOperationException("Geen documentstatus mappings gevonden. Configureer eerst de documentstatus mappings.");
        }

        // Get all DET document statuses from the API
        var allDetDocumentstatuses = await detApiClient.GetAllDocumentstatussen();

        // Find unmapped DET document statuses
        var unmappedStatuses = allDetDocumentstatuses
            .Where(status => !documentstatusMappings.ContainsKey(status.Naam))
            .Select(s => s.Naam)
            .ToList();

        if (unmappedStatuses.Count > 0)
        {
            throw new InvalidOperationException(
                $"De volgende DET documentstatussen zijn niet gekoppeld: {string.Join(", ", unmappedStatuses)}. " +
                "Configureer eerst alle documentstatus mappings in de instellingen.");
        }

        // Validate all mapped OZ statuses are valid according to DocumentStatus enum
        var invalidMappings = documentstatusMappings
            .Where(m => !Enum.IsDefined(typeof(DocumentStatus), m.Value))
            .ToList();

        if (invalidMappings.Count > 0)
        {
            var invalidDetails = string.Join(", ", invalidMappings.Select(m => $"'{m.Key}' -> '{m.Value}'"));
            var validValues = string.Join(", ", Enum.GetNames(typeof(DocumentStatus)));
            throw new InvalidOperationException(
                $"Ongeldige OpenZaak documentstatussen gevonden in mappings: {invalidDetails}. " +
                $"Geldige waarden zijn: {validValues}");
        }

        return documentstatusMappings;
    }
}
