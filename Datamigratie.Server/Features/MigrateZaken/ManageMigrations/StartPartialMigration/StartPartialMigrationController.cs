using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartPartialMigration.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartPartialMigration;

[ApiController]
[Route("api/migration")]
public class StartPartialMigrationController(
    MigrationWorkerState workerState,
    IMigrationBackgroundTaskQueue backgroundTaskQueue,
    IBuildMigrationQueueItemService buildMigrationQueueItemService) : ControllerBase
{
    [HttpPost("start-partial")]
    public async Task<ActionResult> StartPartialMigration([FromBody] StartPartialMigrationRequest request)
    {
        if (workerState.IsWorking)
            return Conflict(new { message = "Er loopt al een migratie." });

        try
        {
            var queueItem = await buildMigrationQueueItemService.BuildAsync(request.DetZaaktypeId, MigrationType.Partial);
            await backgroundTaskQueue.QueueMigrationAsync(queueItem);
        }
        catch (Exception e)
        {
            return Conflict(new { message = e.Message });
        }

        return Ok();
    }
}
