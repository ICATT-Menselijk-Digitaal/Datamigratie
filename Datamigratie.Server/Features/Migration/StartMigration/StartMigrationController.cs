using Datamigratie.Common.Services.Det;
using Datamigratie.Data;
using Datamigratie.Server.Features.Migration.StartMigration.Models;
using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migration.StartMigration.Services;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Datamigratie.Server.Features.StatusMapping.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migration.StartMigration;

[ApiController]
[Route("api/migration")]
public class StartMigrationController(
    IStartMigrationService startMigrationService, 
    MigrationWorkerState workerState, 
    IDetApiClient detApiClient, 
    IMigrationBackgroundTaskQueue backgroundTaskQueue, 
    DatamigratieDbContext context,
    IStatusMappingService statusMappingService) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult> StartMigration([FromBody] StartMigrationRequest request)
    {
        // perform some validation so the frontend gets quick feedback if something is wrong
        if (workerState.IsWorking)
        {
            return Conflict(new { message = "A migration is already in progress." });
        }

        // validating all statuses are mapped before starting migration
        var allStatusesMapped = await statusMappingService.AreAllStatusesMapped(request.DetZaaktypeId);
        if (!allStatusesMapped)
        {
            return BadRequest(new { message = "Not all DET statuses have been mapped to OZ statuses. Please configure status mappings first." });
        }

        await backgroundTaskQueue.QueueMigrationAsync(new MigrationQueueItem {DetZaaktypeId = request.DetZaaktypeId });
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<MigrationStatusResponse>> GetMigration()
    {
        if (!workerState.IsWorking)
        {
            return Ok(new MigrationStatusResponse() { Status = ServiceMigrationStatus.None });
        }

        if (workerState.DetZaaktypeId == null)
        {
            throw new InvalidDataException("Worker is running a migration without a DetZaaktypeId.");
        }

        return new MigrationStatusResponse()
        {
            Status = ServiceMigrationStatus.InProgress,
            DetZaaktypeId = workerState.DetZaaktypeId,
        };
    }
}
