using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migration.Services;
using Datamigratie.Server.Features.Migration.StartMigration.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migration.StartMigration;

[ApiController]
[Route("api/migration")]
public class StartMigrationController : ControllerBase
{
    private readonly DatamigratieDbContext _context;
    private readonly IMigrationProcessor _migrationProcessor;
    private readonly ILogger<StartMigrationController> _logger;

    public StartMigrationController(DatamigratieDbContext context, IMigrationProcessor migrationProcessor, ILogger<StartMigrationController> logger)
    {
        _context = context;
        _migrationProcessor = migrationProcessor;
        _logger = logger;
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

            var migration = new MigrationTracker
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

            _context.MigrationTrackers.Add(migration);
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
