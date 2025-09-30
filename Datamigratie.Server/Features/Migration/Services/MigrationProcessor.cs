using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.Services;

public interface IMigrationProcessor
{
    Task ProcessMigrationAsync(int migrationId, CancellationToken cancellationToken = default);
    Task<bool> IsMigrationCurrentlyRunningAsync();
    Task<MigrationTracker?> GetCurrentlyRunningMigrationAsync();
    Task RunMigrationAsync();
    void TriggerMigration();
}

public class MigrationProcessor : IHostedService, IMigrationProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MigrationProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer? _timer;

    public MigrationProcessor(IServiceProvider serviceProvider, ILogger<MigrationProcessor> logger, IServiceScopeFactory scopeFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Migration Hosted Service initialized.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Migration Hosted Service shutting down.");
        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Dispose();
        return Task.CompletedTask;
    }


    public async Task<bool> IsMigrationCurrentlyRunningAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatamigratieDbContext>();
        
        try
        {
            var runningMigration = await context.MigrationTrackers
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
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatamigratieDbContext>();
        
        try
        {
            var runningMigration = await context.MigrationTrackers
                .SingleOrDefaultAsync(m => m.Status == MigrationStatus.InProgress);

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
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatamigratieDbContext>();
        
        var migration = await context.MigrationTrackers.FindAsync(migrationId, cancellationToken);
        if (migration == null)
        {
            _logger.LogWarning("Migration with ID {MigrationId} not found", migrationId);
            return;
        }

        try
        {
            await UpdateMigrationStatusAsync(context, migration, MigrationStatus.InProgress);
            migration.StartedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Starting migration {MigrationId} for ZaaktypeId {ZaaktypeId}", 
                migrationId, migration.ZaaktypeId);

            await PerformMockMigrationAsync(context, migration, cancellationToken);

            await UpdateMigrationStatusAsync(context, migration, MigrationStatus.Completed);
            migration.CompletedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Completed migration {MigrationId} for ZaaktypeId {ZaaktypeId}. " +
                                 "Processed: {ProcessedRecords}, Successful: {SuccessfulRecords}, Failed: {FailedRecords}",
                migrationId, migration.ZaaktypeId, migration.ProcessedRecords, 
                migration.SuccessfulRecords, migration.FailedRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing migration {MigrationId}", migrationId);
            
            await UpdateMigrationStatusAsync(context, migration, MigrationStatus.Failed, ex.Message);
            migration.CompletedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            
            throw;
        }
    }

    private async Task PerformMockMigrationAsync(DatamigratieDbContext context, MigrationTracker migration, CancellationToken cancellationToken)
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
                await UpdateMigrationStatusAsync(context, migration, MigrationStatus.Cancelled);
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
                await context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Migration {MigrationId} progress: {ProcessedRecords}/{TotalRecords} ({Percentage:F1}%)", 
                    migration.Id, migration.ProcessedRecords, migration.TotalRecords,
                    (double)migration.ProcessedRecords / migration.TotalRecords * 100);
            }
        }

        migration.LastUpdated = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
    }

    private Task UpdateMigrationStatusAsync(DatamigratieDbContext context, MigrationTracker migration, MigrationStatus status, string? errorMessage = null)
    {
        migration.Status = status;
        migration.LastUpdated = DateTime.UtcNow;
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            migration.ErrorMessage = errorMessage.Length > 1000 ? errorMessage[..1000] : errorMessage;
        }
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Fire-and-forget trigger for immediate migration processing
    /// </summary>
    public void TriggerMigration()
    {
        Task.Run(async () =>
        {
            try
            {
                _logger.LogInformation("Migration triggered at {time}", DateTime.UtcNow);
                await RunMigrationAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Migration trigger failed");
            }
        });
    }

    // 🔥 Custom method you can call from a controller
    public async Task RunMigrationAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatamigratieDbContext>();

        try
        {
            // Check if there's already a migration running
            var isRunning = await IsMigrationCurrentlyRunningAsync();
            if (isRunning)
            {
                _logger.LogDebug("Migration is already running, skipping this cycle");
                return;
            }

            // Get the next pending migration
            var pendingMigration = await context.MigrationTrackers
                .Where(m => m.Status == MigrationStatus.Pending)
                .OrderBy(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            if (pendingMigration == null)
            {
                _logger.LogDebug("No pending migrations found");
                return;
            }

            _logger.LogInformation("Processing pending migration {MigrationId} for ZaaktypeId {ZaaktypeId}", 
                pendingMigration.Id, pendingMigration.ZaaktypeId);

            await ProcessMigrationAsync(pendingMigration.Id);
            
            _logger.LogInformation("Migration finished at {time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing pending migrations");
        }
    }
}
