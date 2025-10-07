using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migration.Services;
using Datamigratie.Server.Features.Migration.StartMigration.Models;
using Datamigratie.Server.Features.Migration.Workers;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migration.StartMigration;

[ApiController]
[Route("api/migration")]
public class StartMigrationController : ControllerBase
{
    private readonly DatamigratieDbContext _context;
    private readonly IMigrationProcessor _migrationProcessor;
    private readonly ILogger<StartMigrationController> _logger;
    private readonly MigrationWorker _worker;
    private readonly IBackgroundTaskQueue _taskQueue;



    public StartMigrationController(IBackgroundTaskQueue backgroundTaskQueue, MigrationWorker worker, DatamigratieDbContext context, IMigrationProcessor migrationProcessor, ILogger<StartMigrationController> logger)
    {
        _context = context;
        _migrationProcessor = migrationProcessor;
        _logger = logger;
        _worker = worker;
        _taskQueue = backgroundTaskQueue;
    }

    [HttpPost("start1")]
    public async Task<ActionResult<StartMigrationResponse>> StartMigration1()
    {
        await _taskQueue.QueueBackgroundWorkItemAsync(_worker.StartAsync);
        await _worker.StartAsync(CancellationToken.None);
        return Ok();
    }

    private async ValueTask BuildWorkItem(CancellationToken token)
    {
        // Simulate three 5-second tasks to complete
        // for each enqueued work item

        int delayLoop = 0;
        var guid = Guid.NewGuid().ToString();

        _logger.LogInformation("Queued Background Task {Guid} is starting.", guid);

        while (!token.IsCancellationRequested && delayLoop < 3)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if the Delay is cancelled
            }

            delayLoop++;

            _logger.LogInformation("Queued Background Task {Guid} is running. "
                                   + "{DelayLoop}/3", guid, delayLoop);
        }

        if (delayLoop == 3)
        {
            _logger.LogInformation("Queued Background Task {Guid} is complete.", guid);
        }
        else
        {
            _logger.LogInformation("Queued Background Task {Guid} was cancelled.", guid);
        }
    }

    [HttpPost("start")]
    public async Task<ActionResult<StartMigrationResponse>> StartMigration([FromBody] StartMigrationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isRunning = await _migrationProcessor.IsMigrationCurrentlyRunningAsync();
            if (isRunning)
            {
                var runningMigration = await _migrationProcessor.GetCurrentlyRunningMigrationAsync();
                _logger.LogWarning("Cannot start migration - another migration is already running (ID: {MigrationId})", 
                    runningMigration?.Id);
                
                return Ok(new StartMigrationResponse
                {
                    MigrationId = 0,
                    DetZaaktypeId = request.DetZaaktypeId,
                    Status = MigrationStatus.InProgress,
                    CreatedAt = DateTime.UtcNow,
                    Message = "Migration is already in progress. Only one migration can run at a time."
                });
            }

            var migration = new Data.Entities.Migration
            {
                DetZaaktypeId = request.DetZaaktypeId,
                Status = MigrationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                TotalRecords = 0,
                ProcessedRecords = 0,
                SuccessfulRecords = 0,
                FailedRecords = 0
            };

            _context.Migrations.Add(migration);
            await _context.SaveChangesAsync();
 
            // Fire-and-forget trigger
            _migrationProcessor.TriggerMigration();

            _logger.LogInformation("Migration created with ID {MigrationId} for ZaaktypeId {ZaaktypeId}", 
                migration.Id, request.DetZaaktypeId);

            return Accepted(new StartMigrationResponse
            {
                MigrationId = migration.Id,
                DetZaaktypeId = migration.DetZaaktypeId,
                Status = migration.Status,
                CreatedAt = migration.CreatedAt,
                Message = "Migration started in background"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in StartMigration endpoint for ZaaktypeId {ZaaktypeId}", 
                request.DetZaaktypeId);
            return StatusCode(500, new { message = "An error occurred while starting the migration" });
        }
    }
}
