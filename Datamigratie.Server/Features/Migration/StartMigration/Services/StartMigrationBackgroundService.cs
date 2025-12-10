using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.State;

namespace Datamigratie.Server.Features.Migration.StartMigration.Services
{
    /// <summary>
    /// Code based on:
    /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-9.0&tabs=visual-studio
    /// </summary>
    public class StartMigrationBackgroundService(IServiceScopeFactory scopeFactory, IMigrationBackgroundTaskQueue taskQueue,
        ILogger<StartMigrationBackgroundService> logger, MigrationWorkerState workerState) : BackgroundService
    {
        public IMigrationBackgroundTaskQueue TaskQueue { get; } = taskQueue;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation(
                $"Start Migration Service is running.");

            await BackgroundProcessing(stoppingToken);
        }
         
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {


            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem =
                    await TaskQueue.DequeueMigrationAsync(stoppingToken);

                    using var scope = scopeFactory.CreateScope();
                    // fetch the scoped service manually through the service provider
                    // scoped services cannot be injected directly into the constructor because of the nature of the background service
                    // see: https://learn.microsoft.com/en-us/dotnet/core/extensions/scoped-service
                    var migrationService = scope.ServiceProvider.GetRequiredService<IStartMigrationService>();

                try
                {
                    // set worker state for other threads to read from
                    workerState.DetZaaktypeId = workItem.DetZaaktypeId;
                    workerState.IsWorking = true; 

                    await migrationService.PerformMigrationAsync(workItem, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        "Error occurred executing migration for DET Zaaktype Id {DetZaaktypeId}.", workItem.DetZaaktypeId);
                }
                finally
                {
                    // reset after task finishes or crashes
                    workerState.IsWorking = false;
                    workerState.DetZaaktypeId = null;
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
