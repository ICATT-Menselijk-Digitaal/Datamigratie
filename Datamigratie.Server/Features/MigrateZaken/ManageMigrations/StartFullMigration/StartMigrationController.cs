using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration;

[ApiController]
[Route("api/migration")]
public class StartFullMigrationController(
    MigrationWorkerState workerState,
    IMigrationBackgroundTaskQueue backgroundTaskQueue,
    IBuildMigrationQueueItemService buildMigrationQueueItemService) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult> StartFullMigration([FromBody] StartMigrationRequest request)
    {
        if (workerState.IsWorking)
            return Conflict(new { message = "Er loopt al een migratie." });

        try
        {
            var queueItem = await buildMigrationQueueItemService.ValidateAndBuildAsync(request.DetZaaktypeId, MigrationType.Full);
            await backgroundTaskQueue.QueueMigrationAsync(queueItem);
        }
        catch (Exception e)
        {
            return Conflict(new { message = e.Message });
        }

        return Ok();
    }
}
