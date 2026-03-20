using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartFullMigration;

[ApiController]
[Route("api/migration")]
public class StartFullMigrationController(
    MigrationWorkerState workerState,
    IMigrationBackgroundTaskQueue backgroundTaskQueue,
    IBuildMigrationQueueItemService buildMigrationQueueItemService,
    IDetApiClient detApiClient) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult> StartFullMigration([FromBody] StartMigrationRequest request)
    {
        if (workerState.IsWorking)
            return Conflict(new { message = "Er loopt al een migratie." });

        try
        {
            var zakenSelector = new FullMigrationZakenSelector(detApiClient);
            var queueItem = await buildMigrationQueueItemService.ValidateAndBuildAsync(request.DetZaaktypeId, zakenSelector);
            await backgroundTaskQueue.QueueMigrationAsync(queueItem);
        }
        catch (Exception e)
        {
            return Conflict(new { message = e.Message });
        }

        return Ok();
    }
}
