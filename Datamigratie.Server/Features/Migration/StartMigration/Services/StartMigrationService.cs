using System;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;
using Datamigratie.Server.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.StartMigration.Services;

public interface IStartMigrationService
{
    Task PerformMigrationAsync(MigrationQueueItem migrationQueueItem, CancellationToken stoppingToken);
}

public class StartMigrationService(DatamigratieDbContext context, IDetApiClient detApiClient, ILogger<StartMigrationService> logger, IRandomProvider randomProvider) : IStartMigrationService
{


    public async Task PerformMigrationAsync(MigrationQueueItem migrationQueueItem, CancellationToken stoppingToken) 
    {
        var zaken = await detApiClient.GetZakenByZaaktype(migrationQueueItem.DetZaaktypeId);
        var mapping = await context.Mappings.FirstOrDefaultAsync(m => m.DetZaaktypeId == migrationQueueItem.DetZaaktypeId, stoppingToken);
        var migration = await CreateMigrationAsync(migrationQueueItem, zaken.Count, stoppingToken);

        if (mapping == null)
        {
            await FailMigrationAsync(migration, $"mapping was not found for DetZaaktypeId {migrationQueueItem.DetZaaktypeId}");
            return;
        }

        await ExecuteMigration(migration, zaken, mapping.OzZaaktypeId, stoppingToken);
        await CompleteMigrationAsync(migration);
    }
    private async Task ExecuteMigration(Data.Entities.Migration migration, List<DetZaakMinimal> zaken, Guid openZaaktypeId, CancellationToken ct)
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

            await MigrateSingleZaakAsync(migration, zaak, openZaaktypeId, ct);
            await ReportProgressAsync(migration, ct);
        }
    }

    private async Task ReportProgressAsync(Data.Entities.Migration migration, CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Migration {Id}: {Processed}/{Total} processed ({Percent:F1}%)",
            migration.Id,
            migration.ProcessedRecords,
            migration.TotalRecords,
            (double)migration.ProcessedRecords / migration.TotalRecords * 100);
    }

    private async Task MigrateSingleZaakAsync(Data.Entities.Migration migration, DetZaakMinimal zaak, Guid openZaaktypeId, CancellationToken ct)
    {
        await Task.Delay(randomProvider.Next(5000, 10000), ct); // Replace with actual OZ API call

        if (randomProvider.NextDouble() < 0.9)
            migration.SuccessfulRecords++;
        else
            migration.FailedRecords++;

        migration.ProcessedRecords++;
        migration.LastUpdated = DateTime.UtcNow;
    }

    private async Task CancelMigrationAsync(Data.Entities.Migration migration)
    {
        await UpdateMigrationStatusAsync(migration, MigrationStatus.Cancelled);
    }

    private async Task FailMigrationAsync(Data.Entities.Migration migration, string message)
    {
        await UpdateMigrationStatusAsync(migration, MigrationStatus.Failed, message);
    }

    private async Task UpdateMigrationStatusAsync(Data.Entities.Migration migration, MigrationStatus status, string? errorMessage = null)
    {
        migration.Status = status;
        migration.LastUpdated = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            migration.ErrorMessage = errorMessage.Length > 1000 ? errorMessage[..1000] : errorMessage;
        }

        logger.LogInformation("Migration {Id} has been set to status {Status} with error message: {Message}", migration.Id, status, migration.ErrorMessage);

        await context.SaveChangesAsync();
    }

    private async Task CompleteMigrationAsync(Data.Entities.Migration migration)
    {
        migration.CompletedAt = DateTime.UtcNow;
        await UpdateMigrationStatusAsync(migration, MigrationStatus.Completed);
    }

    private async Task<Data.Entities.Migration> CreateMigrationAsync(MigrationQueueItem queueItem, int totalZaken, CancellationToken ct)
    {
        var migration = new Data.Entities.Migration
        {
            DetZaaktypeId = queueItem.DetZaaktypeId,
            Status = MigrationStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            StartedAt = DateTime.UtcNow,
            TotalRecords = totalZaken
        };

        context.Migrations.Add(migration);
        await context.SaveChangesAsync(ct);
        return migration;
    }
}
