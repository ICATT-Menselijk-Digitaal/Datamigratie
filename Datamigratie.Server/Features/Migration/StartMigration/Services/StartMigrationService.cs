using System;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Datamigratie.Server.Features.MigrateZaak;
using Microsoft.EntityFrameworkCore;
using Datamigratie.Server.Features.Mapping.GlobalMapping.Upsert;
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


    public async Task PerformMigrationAsync(MigrationQueueItem migrationQueueItem, CancellationToken stoppingToken)
    {
        var allZaken = await detApiClient.GetZakenByZaaktype(migrationQueueItem.DetZaaktypeId);

        var closedZaken = allZaken.Where(z => !z.Open).ToList();

        logger.LogInformation(
            "Found {TotalCount} zaken for zaaktype {ZaaktypeId}, {ClosedCount} are closed and will be migrated",
            allZaken.Count,
            migrationQueueItem.DetZaaktypeId,
            closedZaken.Count);

        var migration = await CreateMigrationAsync(migrationQueueItem, closedZaken.Count, stoppingToken);

        workerState.MigrationId = migration.Id;

        // validate RSIN
        var globalmapping = await context.GlobalConfigurations.Select(x=> new GlobalMapping { Rsin = x.Rsin }).SingleAsync(cancellationToken: stoppingToken);


        if (globalmapping == null || !RsinValidator.IsValid(globalmapping?.Rsin))
        {
            var errorMessage = "Kan migratie niet starten: RSIN is niet of niet correct geconfigureerd. Configureer een geldig RSIN op de globale configuratiepagina.";
            await FailMigrationAsync(migration, $"{errorMessage}");
            return;
        }

        var zaakTypeMapping = await context.Mappings.FirstOrDefaultAsync(m => m.DetZaaktypeId == migrationQueueItem.DetZaaktypeId, stoppingToken);

        if (zaakTypeMapping == null)
        {
            await FailMigrationAsync(migration, $"mapping was not found for DetZaaktypeId {migrationQueueItem.DetZaaktypeId}");
            return;
        }

        await ExecuteMigration(migration, closedZaken, zaakTypeMapping.OzZaaktypeId, globalmapping, stoppingToken);
        await CompleteMigrationAsync(migration);
    }
    private async Task ExecuteMigration(Data.Entities.Migration migration, List<DetZaakMinimal> zaken, Guid openZaaktypeId, GlobalMapping? globalmapping, CancellationToken ct)
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

            await MigrateSingleZaakAsync(migration, zaak, openZaaktypeId, globalmapping, ct);
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

    private async Task MigrateSingleZaakAsync(Data.Entities.Migration migration, DetZaakMinimal zaakMinimal, Guid openZaaktypeId, GlobalMapping? globalmapping, CancellationToken ct)
    {
        MigrationRecord record;
        
        try
        {
            logger.LogDebug("Fetching full details for zaak {Zaaknummer}", zaakMinimal.FunctioneleIdentificatie);
            var fullZaak = await detApiClient.GetZaakByZaaknummer(zaakMinimal.FunctioneleIdentificatie);
            
            if (fullZaak == null)
            {
                throw new InvalidOperationException($"Could not fetch full details for zaak {zaakMinimal.FunctioneleIdentificatie}");
            }

       

           var migrateZaakMappingModel  = new MigrateZaakMappingModel {  OpenZaaktypeId = openZaaktypeId,  Rsin = globalmapping?.Rsin  };

            var result = await migrateZaakService.MigrateZaak(fullZaak, migrateZaakMappingModel, ct);
            
            // create migration record based on result
            if (result.IsSuccess)
            {
                migration.SuccessfulRecords++;
                record = new MigrationRecord
                {
                    MigrationId = migration.Id,
                    Migration = migration,
                    DetZaaknummer = zaakMinimal.FunctioneleIdentificatie,
                    OzZaaknummer = result.Zaaknummer,
                    IsSuccessful = true,
                    ProcessedAt = DateTime.UtcNow
                };
                
                logger.LogInformation("Successfully migrated zaak {DetZaaknummer} to {OzZaaknummer}", 
                    zaakMinimal.FunctioneleIdentificatie, result.Zaaknummer);
            }
            else
            {
                migration.FailedRecords++;
                record = new MigrationRecord
                {
                    MigrationId = migration.Id,
                    Migration = migration,
                    DetZaaknummer = zaakMinimal.FunctioneleIdentificatie,
                    IsSuccessful = false,
                    ErrorTitle = result.Message,
                    ErrorDetails = result.Details,
                    StatusCode = result.Statuscode,
                    ProcessedAt = DateTime.UtcNow
                };
                
                logger.LogWarning("Failed to migrate zaak {DetZaaknummer}: {Message} - {Details}", 
                    zaakMinimal.FunctioneleIdentificatie, result.Message, result.Details);
            }
        }
        catch (Exception ex)
        {
            migration.FailedRecords++;
            record = new MigrationRecord
            {
                MigrationId = migration.Id,
                Migration = migration,
                DetZaaknummer = zaakMinimal.FunctioneleIdentificatie,
                IsSuccessful = false,
                ErrorTitle = "Unexpected error during migration",
                ErrorDetails = ex.Message,
                StatusCode = 500,
                ProcessedAt = DateTime.UtcNow
            };
            
            logger.LogError(ex, "Unexpected error while migrating zaak {DetZaaknummer}", 
                zaakMinimal.FunctioneleIdentificatie);
        }
        
        context.MigrationRecords.Add(record);
        
        // update migration progress counters
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
