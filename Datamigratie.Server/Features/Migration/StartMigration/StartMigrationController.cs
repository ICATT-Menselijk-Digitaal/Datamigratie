using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
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
    private readonly IMigrationBackgroundTaskQueue _taskQueue;
    private readonly MigrationWorkerStatus _workerStatus;
    private readonly IDetApiClient _detApiClient;




    public StartMigrationController(MigrationWorkerStatus workerStatus, IDetApiClient detApiClient, IMigrationBackgroundTaskQueue backgroundTaskQueue, DatamigratieDbContext context, IMigrationProcessor migrationProcessor, ILogger<StartMigrationController> logger)
    {
        _context = context;
        _migrationProcessor = migrationProcessor;
        _logger = logger;
        _taskQueue = backgroundTaskQueue;
        _workerStatus = workerStatus;
        _detApiClient = detApiClient;
    }

    [HttpPost("start")]
    public async Task<ActionResult> StartMigration([FromBody] StartMigrationRequest request)
    {
        if (_workerStatus.IsWorking)
        {
            return Conflict(new { message = "A migration is already in progress." });
        }

        var zakenToMigrate = await _detApiClient.GetZakenByZaaktype(request.DetZaaktypeId);

        if (zakenToMigrate.Count == 0)
        {
            return NotFound(new { message = $"No zaken found for DetZaaktypeId {request.DetZaaktypeId}" });
        }

        var mapping = _context.Mappings.FirstOrDefault(m => m.DetZaaktypeId == request.DetZaaktypeId);

        if (mapping == null)
        {
            return NotFound(new { message = $"No mapping found for DetZaaktypeId {request.DetZaaktypeId}" });
        }

        await _taskQueue.QueueMigrationAsync(new MigrationQueueItem {DetZaaktypeId = request.DetZaaktypeId });
        return Ok();
    }

    [HttpGet("status")]
    public ActionResult<bool> Status()
    {
        var status = _workerStatus.IsWorking;
        return Ok(status);
    }

    //[HttpPost("start")]
    //public async Task<ActionResult<StartMigrationResponse>> StartMigration([FromBody] StartMigrationRequest request)
    //{
    //    try
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }

    //        var isRunning = await _migrationProcessor.IsMigrationCurrentlyRunningAsync();
    //        if (isRunning)
    //        {
    //            var runningMigration = await _migrationProcessor.GetCurrentlyRunningMigrationAsync();
    //            _logger.LogWarning("Cannot start migration - another migration is already running (ID: {MigrationId})", 
    //                runningMigration?.Id);
                
    //            return Ok(new StartMigrationResponse
    //            {
    //                MigrationId = 0,
    //                DetZaaktypeId = request.DetZaaktypeId,
    //                Status = MigrationStatus.InProgress,
    //                CreatedAt = DateTime.UtcNow,
    //                Message = "Migration is already in progress. Only one migration can run at a time."
    //            });
    //        }

    //        var migration = new Data.Entities.Migration
    //        {
    //            DetZaaktypeId = request.DetZaaktypeId,
    //            Status = MigrationStatus.Pending,
    //            CreatedAt = DateTime.UtcNow,
    //            LastUpdated = DateTime.UtcNow,
    //            TotalRecords = 0,
    //            ProcessedRecords = 0,
    //            SuccessfulRecords = 0,
    //            FailedRecords = 0
    //        };

    //        _context.Migrations.Add(migration);
    //        await _context.SaveChangesAsync();
 
    //        // Fire-and-forget trigger
    //        _migrationProcessor.TriggerMigration();

    //        _logger.LogInformation("Migration created with ID {MigrationId} for ZaaktypeId {ZaaktypeId}", 
    //            migration.Id, request.DetZaaktypeId);

    //        return Accepted(new StartMigrationResponse
    //        {
    //            MigrationId = migration.Id,
    //            DetZaaktypeId = migration.DetZaaktypeId,
    //            Status = migration.Status,
    //            CreatedAt = migration.CreatedAt,
    //            Message = "Migration started in background"
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error in StartMigration endpoint for ZaaktypeId {ZaaktypeId}", 
    //            request.DetZaaktypeId);
    //        return StatusCode(500, new { message = "An error occurred while starting the migration" });
    //    }
    //}
}
