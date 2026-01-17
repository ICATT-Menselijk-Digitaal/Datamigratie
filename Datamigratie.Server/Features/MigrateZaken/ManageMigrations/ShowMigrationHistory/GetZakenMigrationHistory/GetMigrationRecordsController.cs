using Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZakenMigrationHistory.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZakenMigrationHistory.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZakenMigrationHistory;

[ApiController]
[Route("api/migration")]
public class GetMigrationRecordsController(IGetMigrationRecordsService getMigrationRecordsService) : ControllerBase
{
    [HttpGet("{migrationId}/records")]
    public async Task<ActionResult<List<MigrationRecordItem>>> GetMigrationRecords(int migrationId)
    {
        var records = await getMigrationRecordsService.GetMigrationRecords(migrationId);
        return Ok(records);
    }
}
