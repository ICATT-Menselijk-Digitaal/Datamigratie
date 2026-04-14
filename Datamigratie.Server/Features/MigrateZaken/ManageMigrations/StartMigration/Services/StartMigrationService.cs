using System.Diagnostics;
using System.Diagnostics.Metrics;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;

public interface IStartMigrationService
{
    Task PerformMigrationAsync(MigrationQueueItem migrationQueueItem, CancellationToken stoppingToken);
    Task FailMigrationWithExceptionAsync(int migrationId, Exception exception);
}

public class StartMigrationService(
    DatamigratieDbContext context,
    ILogger<StartMigrationService> logger,
    IMigrateZaakService migrateZaakService,
    MigrationWorkerState workerState) : IStartMigrationService
{
    private static readonly Meter Meter = new("Datamigratie.Server");

    // Duration of a full migration run (all zaken for one zaaktype)
    private static readonly Histogram<double> MigrationDurationHistogram =
        Meter.CreateHistogram<double>("migration.duration", "ms", "Duration of a full migration run");

    private const int MaxErrorMessageLength = 1000;

    public async Task PerformMigrationAsync(MigrationQueueItem migrationQueueItem, CancellationToken stoppingToken)
    {
        var migration = await CreateMigrationAsync(migrationQueueItem, stoppingToken);

        workerState.MigrationId = migration.Id;

        List<DetZaakMinimal> closedZaken = [.. await migrationQueueItem.ZakenSelector.SelectZakenAsync(migrationQueueItem.DetZaaktypeId, stoppingToken)];

        await UpdateMigrationTotalRecordsAsync(migration, closedZaken.Count, stoppingToken);

        var migrationSw = Stopwatch.StartNew();
        await ExecuteMigration(migration, closedZaken, migrationQueueItem, stoppingToken);
        migrationSw.Stop();
        MigrationDurationHistogram.Record(migrationSw.Elapsed.TotalMilliseconds);
        await CompleteMigrationAsync(migration);
    }
    private async Task ExecuteMigration(Data.Entities.Migration migration, List<DetZaakMinimal> zaken, MigrationQueueItem queueItem, CancellationToken ct)
    {
        logger.LogInformation("Starting migration {Id} for DET ZaaktypeId {DetZaaktypeId} with zaken count {Count} to migrate",
            migration.Id, migration.DetZaaktypeId, zaken.Count);

        foreach (var zaak in zaken)
        {
            if (ct.IsCancellationRequested)
            {
                await CancelMigrationAsync(migration);
                return;
            }

            await MigrateSingleZaakAsync(migration, zaak, queueItem, ct);
            await ReportProgressAsync(migration, ct);
        }
    }

    private async Task ReportProgressAsync(Migration migration, CancellationToken ct)
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

    private async Task MigrateSingleZaakAsync(Migration migration, DetZaakMinimal zaakMinimal, MigrationQueueItem queueItem, CancellationToken ct)
    {
        var mapping = new Mappers
        {
            ResultaatMapper = queueItem.ResultaatMapper,
            StatusMapper = queueItem.StatusMapper,
            ZaakMapper = queueItem.ZaakMapper,
            DocumentMapper = queueItem.DocumentMapper,
            BesluitMapper = queueItem.BesluitMapper,
            PdfMapper = queueItem.PdfMapper,
            RolMapper = queueItem.RolMapper
        };

        var result = await migrateZaakService.MigrateZaak(zaakMinimal.FunctioneleIdentificatie, mapping, ct);

        var record = CreateMigrationRecord(migration, zaakMinimal.FunctioneleIdentificatie, result);
        context.MigrationRecords.Add(record);
        migration.ProcessedRecords++;
        migration.LastUpdated = DateTime.UtcNow;
    }

    private MigrationRecord CreateMigrationRecord(Migration migration, string detZaaknummer, MigrateZaakResult result)
    {
        if (result.IsSuccess)
        {
            logger.LogInformation("Successfully migrated zaak {DetZaaknummer} to {OzZaaknummer}", detZaaknummer, result.Zaaknummer);
            migration.SuccessfulRecords++;
            return CreateSuccessfulMigrationRecord(migration, detZaaknummer, result);
        }
        else
        {
            logger.LogWarning("Failed to migrate zaak {DetZaaknummer} to OpenZaak. {ErrorTitle}, (Status: {StatusCode})",
                detZaaknummer, result.Message, result.Statuscode);
            migration.FailedRecords++;
            return CreateFailedMigrationRecord(migration, detZaaknummer, result.Message, result.Details, result.Statuscode);
        }
    }

    private static MigrationRecord CreateSuccessfulMigrationRecord(Migration migration, string detZaaknummer, MigrateZaakResult result)
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

    private static MigrationRecord CreateFailedMigrationRecord(Migration migration, string detZaaknummer, string? errorTitle, string? errorDetails, int? statusCode)
    {
        return new MigrationRecord
        {
            MigrationId = migration.Id,
            Migration = migration,
            IsSuccessful = false,
            DetZaaknummer = detZaaknummer,
            ErrorTitle = errorTitle,
            ErrorDetails = errorDetails?.Length > MigrationRecord.MaxErrorDetailsLength ? errorDetails[..MigrationRecord.MaxErrorDetailsLength] : errorDetails,
            StatusCode = statusCode,
            ProcessedAt = DateTime.UtcNow
        };
    }

    private async Task CancelMigrationAsync(Migration migration)
    {
        await UpdateMigrationStatusAsync(migration, MigrationStatus.Cancelled);
    }

    private async Task FailMigrationAsync(Migration migration, string message)
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

    private async Task UpdateMigrationStatusAsync(Migration migration, MigrationStatus status, string? errorMessage = null)
    {
        migration.Status = status;
        migration.LastUpdated = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            migration.ErrorMessage = errorMessage.Length > Migration.MaxErrorMessageLength
                ? errorMessage[..Migration.MaxErrorMessageLength]
                : errorMessage;
        }

        logger.LogInformation("Migration {Id} has been set to status {Status} with error message: {Message}", migration.Id, status, migration.ErrorMessage);

        await context.SaveChangesAsync();
    }

    private async Task CompleteMigrationAsync(Migration migration)
    {
        migration.CompletedAt = DateTime.UtcNow;
        await UpdateMigrationStatusAsync(migration, MigrationStatus.Completed);
    }

    private async Task UpdateMigrationTotalRecordsAsync(Migration migration, int totalRecords, CancellationToken stoppingToken)
    {
        migration.TotalRecords = totalRecords;
        migration.LastUpdated = DateTime.UtcNow;
        await context.SaveChangesAsync(stoppingToken);
    }

    private async Task<Migration> CreateMigrationAsync(MigrationQueueItem queueItem, CancellationToken ct)
    {
        var migration = new Migration
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
