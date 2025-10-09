using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.State;

namespace Datamigratie.Server.Features.Migration.StartMigration.Services
{
    /// <summary>
    /// Code based on:
    /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-9.0&tabs=visual-studio
    /// </summary>
    public class StartMigrationBackgroundService : BackgroundService
    {
        private readonly ILogger<StartMigrationBackgroundService> _logger;

        private readonly MigrationWorkerStatus _workerStatus;

        private readonly IServiceScopeFactory _scopeFactory;


        public StartMigrationBackgroundService(IServiceScopeFactory scopeFactory, IMigrationBackgroundTaskQueue taskQueue,
            ILogger<StartMigrationBackgroundService> logger, MigrationWorkerStatus workerStatus)
        {
            TaskQueue = taskQueue;
            _logger = logger;
            _workerStatus = workerStatus;
            _scopeFactory = scopeFactory;
        }

        public IMigrationBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }
         
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {

            using var scope = _scopeFactory.CreateScope();
            var migrationService = scope.ServiceProvider.GetRequiredService<IStartMigrationService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem =
                    await TaskQueue.DequeueMigrationAsync(stoppingToken);

                try
                {
                    _workerStatus.IsWorking = true; // set flag before starting
                    await migrationService.PerformMigrationAsync(stoppingToken, workItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(workItem));
                }
                finally
                {
                    _workerStatus.IsWorking = false; // reset after task finishes or crashes
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
