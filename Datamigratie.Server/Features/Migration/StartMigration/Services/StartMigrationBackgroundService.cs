using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;
using Datamigratie.Server.Features.Mapping.GlobalMapping;
using Datamigratie.Server.Features.Migration.StartMigration.Models;

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
                var workItem = await TaskQueue.DequeueMigrationAsync(stoppingToken);

                using var scope = scopeFactory.CreateScope();
                // fetch the scoped service manually through the service provider
                // scoped services cannot be injected directly into the constructor because of the nature of the background service
                // see: https://learn.microsoft.com/en-us/dotnet/core/extensions/scoped-service
                var migrationService = scope.ServiceProvider.GetRequiredService<IStartMigrationService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<DatamigratieDbContext>();
                var loggerForValidator = scope.ServiceProvider.GetRequiredService<ILogger<StartMigrationBackgroundService>>();

                try
                {
                    // Get and validate GlobalMapping before starting migration
                    var globalmapping = await GetAndValidateGlobalMappingAsync(dbContext, loggerForValidator, stoppingToken);

                    workItem.GlobalMapping = globalmapping;

                    // set worker state for other threads to read from
                    workerState.DetZaaktypeId = workItem.DetZaaktypeId;
                    workerState.IsWorking = true; 

                    await migrationService.PerformMigrationAsync(workItem, stoppingToken);
                }
                catch (InvalidOperationException)
                {
                    // Global configuration not found - stop processing
                    return;
                }
                catch (ArgumentException)
                {
                    // Invalid RSIN - stop processing
                    return;
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

        private async Task<GlobalMapping> GetAndValidateGlobalMappingAsync(
            DatamigratieDbContext dbContext, 
            ILogger<StartMigrationBackgroundService> validatorLogger, 
            CancellationToken stoppingToken)
        {
            GlobalMapping globalmapping;
            try
            {
                globalmapping = await dbContext.GlobalConfigurations
                    .Select(x => new GlobalMapping { Rsin = x.Rsin })
                    .SingleAsync(cancellationToken: stoppingToken);

                RsinValidator.ValidateRsin(globalmapping.Rsin, validatorLogger);
            }
            catch (InvalidOperationException)
            {
                logger.LogError("Migration cannot start: No global configuration found. Please configure a valid RSIN in the global configuration page.");
                workerState.IsWorking = false;
                throw;
            }
            catch (ArgumentException ex)
            {
                logger.LogError("Migration cannot start: Invalid RSIN - {Message}. Please configure a valid RSIN in the global configuration page.", ex.Message);
                workerState.IsWorking = false;
                throw;
            }

            return globalmapping;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Start Migration Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
