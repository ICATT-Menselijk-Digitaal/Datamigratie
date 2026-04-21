using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Channels;
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
        try
        {
            await ExecuteMigration(migration, closedZaken, migrationQueueItem, stoppingToken);
            migrationSw.Stop();
            MigrationDurationHistogram.Record(migrationSw.Elapsed.TotalMilliseconds);
            await CompleteMigrationAsync(migration);
        }
        catch (OperationCanceledException)
        {
            migrationSw.Stop();
            logger.LogWarning("Migration {MigrationId} was cancelled after {ElapsedMs}ms", migration.Id, migrationSw.ElapsedMilliseconds);

            await UpdateMigrationStatusAsync(migration, MigrationStatus.Cancelled);
        }
    }
    private async Task ExecuteMigration(Data.Entities.Migration migration, List<DetZaakMinimal> zaken, MigrationQueueItem queueItem, CancellationToken ct)
    {
        var concurrencyLimit = Math.Max(1, _migrationOptions.ZaakConcurrencyLimit);

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

        var channel = Channel.CreateUnbounded<MigrationRecord>();

        var sw = Stopwatch.StartNew();

        var readTask = Task.Run(async () =>
        {
            await foreach (var record in channel.Reader.ReadAllAsync(ct))
            {
                context.MigrationRecords.Add(record);
                migration.ProcessedRecords++;
                if (record.IsSuccessful)
                    migration.SuccessfulRecords++;
                else
                    migration.FailedRecords++;
                migration.LastUpdated = DateTime.UtcNow;
                await ReportProgressAsync(ct);
            }
        }, ct);

        await Parallel.ForEachAsync(
            zaken,
            new ParallelOptions { CancellationToken = ct, MaxDegreeOfParallelism = concurrencyLimit },
            async (zaak, parallelCt) =>
            {
                var result = await migrateZaakService.MigrateZaak(zaak.FunctioneleIdentificatie, mapping, parallelCt);
                LogMigrationResult(zaak.FunctioneleIdentificatie, result);
                var record = CreateMigrationRecord(migration.Id, zaak.FunctioneleIdentificatie, result);
                await channel.Writer.WriteAsync(record, parallelCt);
            });

        channel.Writer.Complete();
        await readTask;

        sw.Stop();

        logger.LogInformation(
            "Migration {Id} done — {Processed}/{Total} processed, {Successful} succeeded, {Failed} failed, time elapsed: {Elapsed}ms (concurrency: {Concurrency})",
            migration.Id, migration.ProcessedRecords, migration.TotalRecords ?? 0, migration.SuccessfulRecords, migration.FailedRecords, sw.ElapsedMilliseconds, concurrencyLimit);
    }

    private async Task ReportProgressAsync(CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);
    }

    private void LogMigrationResult(string zaaknummer, MigrateZaakResult result)
    {
        if (result.IsSuccess)
            logger.LogInformation("Successfully migrated zaak {DetZaaknummer} to {OzZaaknummer}", zaaknummer, result.Zaaknummer);
        else
            logger.LogWarning("Failed to migrate zaak {DetZaaknummer} to OpenZaak. {ErrorTitle}, (Status: {StatusCode})",
                zaaknummer, result.Message, result.Statuscode);
    }

    private static MigrationRecord CreateMigrationRecord(int migrationId, string detZaaknummer, MigrateZaakResult result)
    {
        return result.IsSuccess
            ? new MigrationRecord
            {
                MigrationId = migrationId,
                IsSuccessful = true,
                DetZaaknummer = detZaaknummer,
                OzZaaknummer = result.Zaaknummer,
                ProcessedAt = DateTime.UtcNow
            }
            : new MigrationRecord
            {
                MigrationId = migrationId,
                IsSuccessful = false,
                DetZaaknummer = detZaaknummer,
                ErrorTitle = result.Message,
                ErrorDetails = result.Details?.Length > MigrationRecord.MaxErrorDetailsLength
                    ? result.Details[..MigrationRecord.MaxErrorDetailsLength]
                    : result.Details,
                StatusCode = result.Statuscode,
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
