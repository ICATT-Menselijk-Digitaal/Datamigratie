using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.GetMigration.Models;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.GetMigration;

[ApiController]
[Route("api/migration")]
public class GetMigrationController(
    MigrationWorkerState workerState) : ControllerBase
{

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
