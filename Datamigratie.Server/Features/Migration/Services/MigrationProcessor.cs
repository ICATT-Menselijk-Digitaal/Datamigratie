using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.Services;

public interface IMigrationProcessor
{
    Task ProcessMigrationAsync(int migrationId, CancellationToken cancellationToken = default);
    Task<bool> IsMigrationCurrentlyRunningAsync();
    Task<MigrationTracker?> GetCurrentlyRunningMigrationAsync();
}

public class MigrationProcessor : IMigrationProcessor
{
    private readonly DatamigratieDbContext _context;
    private readonly ILogger<MigrationProcessor> _logger;

    public MigrationProcessor(DatamigratieDbContext context, ILogger<MigrationProcessor> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> IsMigrationCurrentlyRunningAsync()
    {
        try
        {
            var runningMigration = await _context.Migrations
                .AnyAsync(m => m.Status == MigrationStatus.InProgress);

            return runningMigration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if migration is currently running");
            return false;
        }
    }

    public async Task<MigrationTracker?> GetCurrentlyRunningMigrationAsync()
    {
        try
        {
            var runningMigration = await _context.Migrations
                .FirstOrDefaultAsync(m => m.Status == MigrationStatus.InProgress);

            return runningMigration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting currently running migration");
            return null;
        }
    }

    public async Task ProcessMigrationAsync(int migrationId, CancellationToken cancellationToken = default)
    {
        var migration = await _context.Migrations.FindAsync(migrationId, cancellationToken);
        if (migration == null)
        {
            _logger.LogWarning("Migration with ID {MigrationId} not found", migrationId);
            return;
        }

        try
        {
            await UpdateMigrationStatusAsync(migration, MigrationStatus.InProgress);
            migration.StartedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Starting migration {MigrationId} for ZaaktypeId {ZaaktypeId}", 
                migrationId, migration.ZaaktypeId);

            await PerformMockMigrationAsync(migration, cancellationToken);

            await UpdateMigrationStatusAsync(migration, MigrationStatus.Completed);
            migration.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Completed migration {MigrationId} for ZaaktypeId {ZaaktypeId}. " +
                                 "Processed: {ProcessedRecords}, Successful: {SuccessfulRecords}, Failed: {FailedRecords}",
                migrationId, migration.ZaaktypeId, migration.ProcessedRecords, 
                migration.SuccessfulRecords, migration.FailedRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing migration {MigrationId}", migrationId);
            
            await UpdateMigrationStatusAsync(migration, MigrationStatus.Failed, ex.Message);
            migration.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            
            throw;
        }
    }

    private async Task PerformMockMigrationAsync(MigrationTracker migration, CancellationToken cancellationToken)
    {
        var random = new Random();
        var totalRecords = random.Next(100, 1000);
        migration.TotalRecords = totalRecords;
        
        _logger.LogInformation("Mock migration for ZaaktypeId {ZaaktypeId} will process {TotalRecords} records", 
            migration.ZaaktypeId, totalRecords);

        for (int i = 0; i < totalRecords; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Migration {MigrationId} was cancelled", migration.Id);
                await UpdateMigrationStatusAsync(migration, MigrationStatus.Cancelled);
                return;
            }

            await Task.Delay(random.Next(10, 50), cancellationToken);

            if (random.NextDouble() < 0.9)
            {
                migration.SuccessfulRecords++;
            }
            else
            {
                migration.FailedRecords++;
            }

            migration.ProcessedRecords++;

            if (migration.ProcessedRecords % 50 == 0)
            {
                migration.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Migration {MigrationId} progress: {ProcessedRecords}/{TotalRecords} ({Percentage:F1}%)", 
                    migration.Id, migration.ProcessedRecords, migration.TotalRecords,
                    (double)migration.ProcessedRecords / migration.TotalRecords * 100);
            }
        }

        migration.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private Task UpdateMigrationStatusAsync(MigrationTracker migration, MigrationStatus status, string? errorMessage = null)
    {
        migration.Status = status;
        migration.LastUpdated = DateTime.UtcNow;
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            migration.ErrorMessage = errorMessage.Length > 1000 ? errorMessage[..1000] : errorMessage;
        }
        
        return Task.CompletedTask;
    }
}
