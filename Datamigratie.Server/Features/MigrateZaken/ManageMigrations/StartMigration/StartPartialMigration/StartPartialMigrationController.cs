using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartPartialMigration.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartPartialMigration;

[ApiController]
[Route("api/migration")]
public class StartPartialMigrationController(
    MigrationWorkerState workerState,
    IMigrationBackgroundTaskQueue backgroundTaskQueue,
    IBuildMigrationQueueItemService buildMigrationQueueItemService,
    IServiceScopeFactory scopeFactory,
    IDetApiClient detApiClient) : ControllerBase
{
    [HttpPost("startpartial")]
    public async Task<ActionResult> StartPartialMigration([FromBody] StartPartialMigrationRequest request)
    {
        if (workerState.IsWorking)
            return Conflict(new { message = "Er loopt al een migratie." });

        try
        {
            var partialMigrationZakenSelector = new PartialMigrationZakenSelector(scopeFactory, detApiClient);
            var queueItem = await buildMigrationQueueItemService.ValidateAndBuildAsync(request.DetZaaktypeId, partialMigrationZakenSelector);
            await backgroundTaskQueue.QueueMigrationAsync(queueItem);
        }
        catch (Exception e)
        {
            return Conflict(new { message = e.Message });
        }

        return Ok();
    }
}
