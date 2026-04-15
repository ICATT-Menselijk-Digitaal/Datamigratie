using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Config;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Models;
using Microsoft.Extensions.Options;

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
    MigrationWorkerState workerState,
    IOptions<MigrationOptions> migrationOptions) : IStartMigrationService
{
    private static readonly Meter Meter = new("Datamigratie.Server");

    // Duration of a full migration run (all zaken for one zaaktype)
    private static readonly Histogram<double> MigrationDurationHistogram =
        Meter.CreateHistogram<double>("migration.duration", "ms", "Duration of a full migration run");

    private readonly MigrationOptions _migrationOptions = migrationOptions.Value;

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
        var concurrencyLimit = Math.Max(1, _migrationOptions.ZaakConcurrencyLimit);

        // Pre-fetch the first informatieobjecttype URI once — same zaaktype for all zaken in this run.
        // This avoids N redundant HTTP calls to OpenZaak. We tolerate failure here: if the zaaktype has
        // no informatieobjecttypen configured yet, we fall back to null and MigrateZaakService will
        // re-fetch per zaak (as it did before). This prevents a single pre-fetch failure from opening
        // the circuit breaker and blocking all subsequent OpenZaak calls in the run.
        Uri? firstInformatieObjectTypeUri = null;
        try
        {
            firstInformatieObjectTypeUri = await migrateZaakService.GetFirstInformatieObjectTypeUriAsync(queueItem.ZaakMapper.OzZaaktypeUrl, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Pre-fetch of first informatieobjecttype URI failed for zaaktype {ZaaktypeUrl}. Will fall back to per-zaak fetch.",
                queueItem.ZaakMapper.OzZaaktypeUrl);
        }

        var completedRecords = new ConcurrentQueue<MigrationRecord>();
        var successCount = 0;
        var failedCount = 0;

        var sw = Stopwatch.StartNew();

        await Parallel.ForEachAsync(
            zaken,
            new ParallelOptions { MaxDegreeOfParallelism = concurrencyLimit, CancellationToken = ct },
            async (zaak, parallelCt) =>
            {
                var (record, isSuccess) = await MigrateSingleZaakAsync(migration.Id, zaak, queueItem, firstInformatieObjectTypeUri, parallelCt);
                completedRecords.Enqueue(record);

                if (isSuccess)
                    Interlocked.Increment(ref successCount);
                else
                    Interlocked.Increment(ref failedCount);
            });

        sw.Stop();

        while (completedRecords.TryDequeue(out var record))
        {
            context.MigrationRecords.Add(record);
        }

        migration.SuccessfulRecords = successCount;
        migration.FailedRecords = failedCount;
        migration.ProcessedRecords = successCount + failedCount;
        migration.LastUpdated = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        logger.LogInformation(
            "[PARALLEL] Migration {Id} done — {Processed}/{Total} processed, {Successful} succeeded, {Failed} failed, total wall time: {Elapsed}ms (concurrency: {Concurrency})",
            migration.Id, migration.ProcessedRecords, migration.TotalRecords ?? 0, successCount, failedCount, sw.ElapsedMilliseconds, concurrencyLimit);
    }

    private async Task<(MigrationRecord record, bool isSuccess)> MigrateSingleZaakAsync(int migrationId, DetZaakMinimal zaakMinimal, MigrationQueueItem queueItem, Uri? firstInformatieObjectTypeUri, CancellationToken ct)
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

        if (result.IsSuccess)
        {
            logger.LogInformation("Successfully migrated zaak {DetZaaknummer} to {OzZaaknummer}", zaakMinimal.FunctioneleIdentificatie, result.Zaaknummer);
            return (CreateSuccessfulMigrationRecord(migrationId, zaakMinimal.FunctioneleIdentificatie, result), true);
        }
        else
        {
            logger.LogWarning("Failed to migrate zaak {DetZaaknummer} to OpenZaak. {ErrorTitle}, (Status: {StatusCode})",
                zaakMinimal.FunctioneleIdentificatie, result.Message, result.Statuscode);
            return (CreateFailedMigrationRecord(migrationId, zaakMinimal.FunctioneleIdentificatie, result.Message, result.Details, result.Statuscode), false);
        }
    }

    private static MigrationRecord CreateSuccessfulMigrationRecord(int migrationId, string detZaaknummer, MigrateZaakResult result)
    {
        return new MigrationRecord
        {
            MigrationId = migrationId,
            IsSuccessful = true,
            DetZaaknummer = detZaaknummer,
            OzZaaknummer = result.Zaaknummer,
            ProcessedAt = DateTime.UtcNow
        };
    }

    private static MigrationRecord CreateFailedMigrationRecord(int migrationId, string detZaaknummer, string? errorTitle, string? errorDetails, int? statusCode)
    {
        return new MigrationRecord
        {
            MigrationId = migrationId,
            IsSuccessful = false,
            DetZaaknummer = detZaaknummer,
            ErrorTitle = errorTitle,
            ErrorDetails = errorDetails?.Length > MigrationRecord.MaxErrorDetailsLength ? errorDetails[..MigrationRecord.MaxErrorDetailsLength] : errorDetails,
            StatusCode = statusCode,
            ProcessedAt = DateTime.UtcNow
        };
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
