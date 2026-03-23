using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration.Models;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration;

[ApiController]
[Route("api/migration")]
public class StartSingleMigrationController(
    MigrationWorkerState workerState,
    IMigrationBackgroundTaskQueue backgroundTaskQueue,
    IBuildMigrationQueueItemService buildMigrationQueueItemService,
    IDetApiClient detApiClient) : ControllerBase
{
    [HttpPost("startsingle")]
    public async Task<ActionResult> StartSingleMigration([FromBody] StartSingleMigrationRequest request)
    {
        if (workerState.IsWorking)
            return Conflict(new { message = "Er loopt al een migratie." });

        try
        {
            var zaak = await detApiClient.GetZaakByZaaknummer(request.Zaaknummer);
            if (zaak is null)
                return UnprocessableEntity(new { message = $"Zaak met zaaknummer '{request.Zaaknummer}' is niet gevonden in DET." });

            if (zaak.Zaaktype?.FunctioneleIdentificatie != request.DetZaaktypeId)
                return UnprocessableEntity(new { message = $"Zaak '{request.Zaaknummer}' behoort niet tot dit zaaktype." });

            if (zaak.Open)
                return UnprocessableEntity(new { message = $"Zaak '{request.Zaaknummer}' is nog open en kan daarom niet gemigreerd worden." });

            var zakenSelector = new SingleZaakSelector(request.Zaaknummer, zaak.Open);
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
