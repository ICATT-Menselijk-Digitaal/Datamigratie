using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services
{
    /// <summary>
    /// Code based on:
    /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-9.0&tabs=visual-studio
    /// </summary>
    public class StartMigrationBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IMigrationBackgroundTaskQueue taskQueue,
        ILogger<StartMigrationBackgroundService> logger, 
        MigrationWorkerState workerState) : BackgroundService
    {
        public IMigrationBackgroundTaskQueue TaskQueue { get; } = taskQueue;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Start Migration Service is running.");

            await BackgroundProcessing(stoppingToken);
        }
         
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueMigrationAsync(stoppingToken);

                using var scope = scopeFactory.CreateScope();
                // fetch the scoped service manually through the service provider
                // scoped services cannot be injected directly into the constructor because of the nature of the background service
                // see: https://learn.microsoft.com/en-us/dotnet/core/extensions/scoped-service
                var migrationService = scope.ServiceProvider.GetRequiredService<IStartMigrationService>();

                try
                {
                    logger.LogInformation(
                        "Start Migration Service is starting migration for DET Zaaktype Id {DetZaaktypeId}.", workItem.DetZaaktypeId);

                    // set worker state for other threads to read from
                    workerState.DetZaaktypeId = workItem.DetZaaktypeId;
                    workerState.IsWorking = true; 

                    await migrationService.PerformMigrationAsync(workItem, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        "Error occurred executing migration for DET Zaaktype Id {DetZaaktypeId}.", workItem.DetZaaktypeId);
                    
                    if (workerState.MigrationId.HasValue)
                    {
                        try
                        {
                            await migrationService.FailMigrationWithExceptionAsync(workerState.MigrationId.Value, ex);
                        }
                        catch (Exception innerEx)
                        {
                            logger.LogError(innerEx, "Failed to update migration {MigrationId} with exception details", workerState.MigrationId.Value);
                        }
                    }
                }
                finally
                {
                    // reset after task finishes or crashes
                    workerState.IsWorking = false;
                    workerState.DetZaaktypeId = null;
                    workerState.MigrationId = null;
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Start Migration Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
