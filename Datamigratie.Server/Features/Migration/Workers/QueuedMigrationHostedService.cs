using Datamigratie.Server.Features.Migration.StartMigration;

namespace Datamigratie.Server.Features.Migration.Workers
{
    public class QueuedMigrationHostedService : BackgroundService
    {
        private readonly ILogger<QueuedMigrationHostedService> _logger;

        private readonly MigrationWorkerStatus _workerStatus;

        private readonly MigrationService _migrationService;

        public QueuedMigrationHostedService(IMigrationBackgroundTaskQueue taskQueue,
            ILogger<QueuedMigrationHostedService> logger, MigrationWorkerStatus workerStatus, MigrationService migrationService)
        {
            TaskQueue = taskQueue;
            _logger = logger;
            _workerStatus = workerStatus;
            _migrationService = migrationService;
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
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem =
                    await TaskQueue.DequeueMigrationAsync(stoppingToken);

                try
                {
                    _workerStatus.IsWorking = true; // set flag before starting
                    await _migrationService.PerformMigrationAsync(stoppingToken, workItem);
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
