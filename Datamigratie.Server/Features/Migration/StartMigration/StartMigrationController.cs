using Datamigratie.Common.Services.Det;
using Datamigratie.Data;
using Datamigratie.Server.Features.Migration.StartMigration.Models;
using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migration.StartMigration;

[ApiController]
[Route("api/migration")]
public class StartMigrationController(MigrationWorkerStatus workerStatus, IDetApiClient detApiClient, IMigrationBackgroundTaskQueue backgroundTaskQueue, DatamigratieDbContext context) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult> StartMigration([FromBody] StartMigrationRequest request)
    {
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

    [HttpGet("status")]
    public ActionResult<bool> Status()
    {
        var status = workerStatus.IsWorking;
        return Ok(status);
    }
}
