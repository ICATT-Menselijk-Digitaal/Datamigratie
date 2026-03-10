using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration;

[ApiController]
[Route("api/migration")]
public class StartMigrationController(
    MigrationWorkerState workerState,
    IMigrationBackgroundTaskQueue backgroundTaskQueue,
    IBuildMigrationQueueItemService buildMigrationQueueItemService) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult> StartMigration([FromBody] StartMigrationRequest request)
    {
        if (workerState.IsWorking)
            return Conflict(new { message = "Er loopt al een migratie." });

        try
        {
            var queueItem = await buildMigrationQueueItemService.BuildAsync(request.DetZaaktypeId, MigrationType.Full);
            await backgroundTaskQueue.QueueMigrationAsync(queueItem);
        }
        catch (Exception e)
        {
            return Conflict(new { message = e.Message });
        }

        return Ok();
    }

    [HttpGet]
    public ActionResult<MigrationStatusResponse> GetMigration()
    {
        if (!workerState.IsWorking)
            return Ok(new MigrationStatusResponse { Status = ServiceMigrationStatus.None });

        if (workerState.DetZaaktypeId == null)
            throw new InvalidDataException("Worker is running a migration without a DetZaaktypeId.");

        return Ok(new MigrationStatusResponse
        {
            Status = ServiceMigrationStatus.InProgress,
            DetZaaktypeId = workerState.DetZaaktypeId
        });
    }
}
