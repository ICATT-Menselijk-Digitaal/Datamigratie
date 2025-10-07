

using System.Formats.Asn1;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migration.Workers;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.StartMigration;

public interface IMigrationService
{
    Task PerformMigrationAsync(CancellationToken stoppingToken, MigrationQueueItem migrationQueueItem);
}

public class MigrationService(DatamigratieDbContext context) : IMigrationService
{
    public async Task PerformMigrationAsync(CancellationToken stoppingToken, MigrationQueueItem migrationQueueItem)
    {
        var migration = await PrepareMigration(migrationQueueItem);

        await ExecuteMigration(migration);

        await FinishMigration(migration);
    }
    private async Task ExecuteMigration(Data.Entities.Migration migration)
    {
        var zakenToMigrate = await context.Zaken
            .Where(z => z.ZaaktypeId == migration.DetZaaktypeId)
            .ToListAsync();

    }

    private async Task FinishMigration(Data.Entities.Migration migration)
    {
        migration.Status = MigrationStatus.Completed;
        migration.CompletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    private async Task<Data.Entities.Migration> PrepareMigration(MigrationQueueItem migrationQueueItem)
    {
        var migration = new Data.Entities.Migration
        {
            DetZaaktypeId = migrationQueueItem.DetZaaktypeId,
            Status = MigrationStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            StartedAt = DateTime.UtcNow
        };

        context.Migrations.Add(migration);
        await context.SaveChangesAsync();
        return migration;
    }
}
