using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.ShowMigrationHistory.GetZakenMigrationHistory.Models;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.ShowMigrationHistory.GetZakenMigrationHistory.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.ShowMigrationHistory.GetZakenMigrationHistory;

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
