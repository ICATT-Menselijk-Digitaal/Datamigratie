using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Datamigratie.Server.Features.MigrateZaak;
using Microsoft.EntityFrameworkCore;
using Datamigratie.Server.Features.Migration.StartMigration.Models;
using Datamigratie.Server.Features.MigrateZaak.Models;

namespace Datamigratie.Server.Features.Migration.StartMigration.Services;

public interface IStartMigrationService
{
    Task PerformMigrationAsync(MigrationQueueItem migrationQueueItem, CancellationToken stoppingToken);
    Task FailMigrationWithExceptionAsync(int migrationId, Exception exception);
}

public class StartMigrationService(
    DatamigratieDbContext context,
    IDetApiClient detApiClient,
    ILogger<StartMigrationService> logger,
    IMigrateZaakService migrateZaakService,
    MigrationWorkerState workerState) : IStartMigrationService
{
    private const int MaxErrorMessageLength = 1000;

    public async Task PerformMigrationAsync(MigrationQueueItem migrationQueueItem, CancellationToken stoppingToken)
    {
        var migration = await CreateMigrationAsync(migrationQueueItem, stoppingToken);

        workerState.MigrationId = migration.Id;

        var allZaken = await detApiClient.GetZakenByZaaktype(migrationQueueItem.DetZaaktypeId);

        var closedZaken = allZaken.Where(z => !z.Open).ToList();

        logger.LogInformation(
            "Found {TotalCount} zaken for zaaktype {ZaaktypeId}, {ClosedCount} are closed and will be migrated",
            allZaken.Count,
            migrationQueueItem.DetZaaktypeId,
            closedZaken.Count);

        await UpdateMigrationTotalRecordsAsync(migration, closedZaken.Count, stoppingToken);

        var zaakTypeMapping = await context.Mappings.FirstOrDefaultAsync(m => m.DetZaaktypeId == migrationQueueItem.DetZaaktypeId, stoppingToken);

        if (zaakTypeMapping == null)
        {
            await FailMigrationAsync(migration, $"mapping was not found for DetZaaktypeId {migrationQueueItem.DetZaaktypeId}");
            return;
        }

        await ExecuteMigration(migration, closedZaken, zaakTypeMapping.OzZaaktypeId, migrationQueueItem.GlobalMapping!, stoppingToken);
        await CompleteMigrationAsync(migration);
    }
    private async Task ExecuteMigration(Data.Entities.Migration migration, List<DetZaakMinimal> zaken, Guid openZaaktypeId, GlobalMapping globalMapping, CancellationToken ct)
    {
        logger.LogInformation("Starting migration {Id} for DET ZaaktypeId {DetZaaktypeId} to OZ ZaaktypeId {OpenZaaktypeId} with zaken count {Count} to migrate", 
            migration.Id, migration.DetZaaktypeId, openZaaktypeId, zaken.Count);

        foreach (var zaak in zaken)
        {
            if (ct.IsCancellationRequested)
            {
                await CancelMigrationAsync(migration);
                return;
            }

            await MigrateSingleZaakAsync(migration, zaak, openZaaktypeId, globalMapping, ct);
            await ReportProgressAsync(migration, ct);
        }
    }

    private async Task ReportProgressAsync(Data.Entities.Migration migration, CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Migration {Id}: {Processed}/{Total} processed ({Percent:F1}%)",
            migration.Id,
            migration.ProcessedRecords,
            migration.TotalRecords ?? 0,
            migration.TotalRecords.HasValue && migration.TotalRecords.Value > 0
                ? (double)migration.ProcessedRecords / migration.TotalRecords.Value * 100
                : 0.0);
    }

    private async Task MigrateSingleZaakAsync(Data.Entities.Migration migration, DetZaakMinimal zaakMinimal, Guid openZaaktypeId, GlobalMapping globalMapping, CancellationToken ct)
    {
        MigrationRecord record;
        try
        {
            var fullZaak = await FetchZaakFromDetAsync(zaakMinimal.FunctioneleIdentificatie);
            var result = await migrateZaakService.MigrateZaak(fullZaak, new MigrateZaakMappingModel {  OpenZaaktypeId = openZaaktypeId,  Rsin = globalMapping.Rsin  }, ct);
            
            record = CreateMigrationRecord(migration, zaakMinimal.FunctioneleIdentificatie, result);
        }
        catch (HttpRequestException httpEx)
        {
            var statusCode = (int?)httpEx.StatusCode ?? 500;
            logger.LogError(httpEx, "Failed to fetch zaak {DetZaaknummer}, Status: {StatusCode}",
                zaakMinimal.FunctioneleIdentificatie, statusCode);

            record = CreateFailedMigrationRecord(migration, zaakMinimal.FunctioneleIdentificatie,
                "De zaak kon niet opgehaald worden uit het bronsysteem.",
                $"HTTP {statusCode}: {httpEx.Message}",
                statusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while migrating zaak {DetZaaknummer}. Exception: {ExceptionType}",
                zaakMinimal.FunctioneleIdentificatie, ex.GetType().Name);

            record = CreateFailedMigrationRecord(migration, zaakMinimal.FunctioneleIdentificatie,
                "Onverwachte fout tijdens migratie",
                ex.Message,
                statusCode: 500);
        }

        context.MigrationRecords.Add(record);
        migration.ProcessedRecords++;
        migration.LastUpdated = DateTime.UtcNow;
    }

    private async Task<DetZaak> FetchZaakFromDetAsync(string zaaknummer)
    {
        logger.LogDebug("Fetching full details for zaak {Zaaknummer} from DET", zaaknummer);

        try
        {
            var zaak = await detApiClient.GetZaakByZaaknummer(zaaknummer);
            return zaak ?? throw new InvalidOperationException($"This zaaknumber '{zaaknummer}' is not found in the DET API");
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to fetch zaak from DET API: {ex.Message}", ex, ex.StatusCode);
        }
    }

    private MigrationRecord CreateMigrationRecord(Data.Entities.Migration migration, string detZaaknummer, MigrateZaakResult result)
    {
        if (result.IsSuccess)
        {
            logger.LogInformation("Successfully migrated zaak {DetZaaknummer} to {OzZaaknummer}", detZaaknummer, result.Zaaknummer);
            migration.SuccessfulRecords++;
            return CreateSuccessfulMigrationRecord(migration, detZaaknummer, result);
        }
        else 
        {
            logger.LogWarning("Failed to migrate zaak {DetZaaknummer} to OpenZaak. {ErrorTitle}: {ErrorDetails} (Status: {StatusCode})",
                detZaaknummer, result.Message, result.Details, result.Statuscode);
            migration.FailedRecords++;
            return CreateFailedMigrationRecord(migration, detZaaknummer, result.Message, result.Details, result.Statuscode);
        }
    }

    private static MigrationRecord CreateSuccessfulMigrationRecord(Data.Entities.Migration migration, string detZaaknummer, MigrateZaakResult result) 
    {
        return new MigrationRecord
        {
            MigrationId = migration.Id,
            Migration = migration,
            IsSuccessful = true,
            DetZaaknummer = detZaaknummer,
            OzZaaknummer = result.Zaaknummer,
            ProcessedAt = DateTime.UtcNow
        };
    }

    private static MigrationRecord CreateFailedMigrationRecord(Data.Entities.Migration migration, string detZaaknummer, string? errorTitle, string? errorDetails, int? statusCode)
    {
        return new MigrationRecord
        {
            MigrationId = migration.Id,
            Migration = migration,
            IsSuccessful = false,
            DetZaaknummer = detZaaknummer,
            ErrorTitle = errorTitle,
            ErrorDetails = errorDetails,
            StatusCode = statusCode,
            ProcessedAt = DateTime.UtcNow
        };
    }

    private async Task CancelMigrationAsync(Data.Entities.Migration migration)
    {
        await UpdateMigrationStatusAsync(migration, MigrationStatus.Cancelled);
    }

    private async Task FailMigrationAsync(Data.Entities.Migration migration, string message)
    {
        await UpdateMigrationStatusAsync(migration, MigrationStatus.Failed, message);
    }

    public async Task FailMigrationWithExceptionAsync(int migrationId, Exception exception)
    {
        var migration = await context.Migrations.FindAsync(migrationId);
        if (migration == null)
        {
            logger.LogError("Migration {Id} not found when trying to set exception", migrationId);
            return;
        }

        var errorMessage = $"{exception.GetType().Name}: {exception.Message}";
        await UpdateMigrationStatusAsync(migration, MigrationStatus.Failed, errorMessage);
        
        logger.LogError(exception, "Migration {Id} failed with exception", migrationId);
    }

    private async Task UpdateMigrationStatusAsync(Data.Entities.Migration migration, MigrationStatus status, string? errorMessage = null)
    {
        migration.Status = status;
        migration.LastUpdated = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            migration.ErrorMessage = errorMessage.Length > MaxErrorMessageLength 
                ? errorMessage[..MaxErrorMessageLength] 
                : errorMessage;
        }

        logger.LogInformation("Migration {Id} has been set to status {Status} with error message: {Message}", migration.Id, status, migration.ErrorMessage);

        await context.SaveChangesAsync();
    }

    private async Task CompleteMigrationAsync(Data.Entities.Migration migration)
    {
        migration.CompletedAt = DateTime.UtcNow;
        await UpdateMigrationStatusAsync(migration, MigrationStatus.Completed);
    }

    private async Task UpdateMigrationTotalRecordsAsync(Data.Entities.Migration migration, int totalRecords, CancellationToken stoppingToken)
    {
        migration.TotalRecords = totalRecords;
        migration.LastUpdated = DateTime.UtcNow;
        await context.SaveChangesAsync(stoppingToken);
    }

    private async Task<Data.Entities.Migration> CreateMigrationAsync(MigrationQueueItem queueItem, CancellationToken ct)
    {
        var migration = new Data.Entities.Migration
        {
            DetZaaktypeId = queueItem.DetZaaktypeId,
            Status = MigrationStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            StartedAt = DateTime.UtcNow
        };

        context.Migrations.Add(migration);
        await context.SaveChangesAsync(ct);
        return migration;
    }
}
