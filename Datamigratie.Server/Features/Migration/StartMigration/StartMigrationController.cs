using Datamigratie.Common.Services.Det;
using Datamigratie.Data;
using Datamigratie.Server.Features.Migration.StartMigration.Models;
using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migration.StartMigration.Services;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migration.StartMigration;

[ApiController]
[Route("api/migration")]
public class StartMigrationController(IStartMigrationService startMigrationService, MigrationWorkerStatus workerStatus, IDetApiClient detApiClient, IMigrationBackgroundTaskQueue backgroundTaskQueue, DatamigratieDbContext context) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult> StartMigration([FromBody] StartMigrationRequest request)
    {
        // perform some validation so the frontend gets quick feedback if something is wrong
        if (workerStatus.IsWorking)
        {
            return Conflict(new { message = "A migration is already in progress." });
        }

        var zakenToMigrate = await detApiClient.GetZakenByZaaktype(request.DetZaaktypeId);

        if (zakenToMigrate.Count == 0)
        {
            return NotFound(new { message = $"No zaken found for DetZaaktypeId {request.DetZaaktypeId}" });
        }

        var mapping = context.Mappings.FirstOrDefault(m => m.DetZaaktypeId == request.DetZaaktypeId);

        if (mapping == null)
        {
            return NotFound(new { message = $"No mapping found for DetZaaktypeId {request.DetZaaktypeId}" });
        }

        await backgroundTaskQueue.QueueMigrationAsync(new MigrationQueueItem {DetZaaktypeId = request.DetZaaktypeId });
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<MigrationStatusResponse>> GetMigration()
    {
        if (!workerStatus.IsWorking)
        {
            return Ok(new MigrationStatusResponse() { Status = ServiceMigrationStatus.None });
        }

        var migration = await startMigrationService.GetRunningMigration();

        if (migration == null)
        {
            // worker is running, but still preparing the migration
            return Ok(new MigrationStatusResponse() { Status = ServiceMigrationStatus.Preparing });
        }

        return new MigrationStatusResponse()
        {
            Status = ServiceMigrationStatus.InProgress,
            DetZaaktypeId = migration.DetZaaktypeId,
        };
    }
}
