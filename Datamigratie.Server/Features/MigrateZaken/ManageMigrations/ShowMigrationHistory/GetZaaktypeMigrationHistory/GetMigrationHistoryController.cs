using Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZaaktypeMigrationHistory.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZaaktypeMigrationHistory.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZaaktypeMigrationHistory;

[ApiController]
[Route("api/migration")]
public class GetMigrationHistoryController(IGetMigrationHistoryService getMigrationHistoryService) : ControllerBase
{
    [HttpGet("history/{detZaaktypeId}")]
    public async Task<ActionResult<List<MigrationHistoryItem>>> GetMigrationHistory(string detZaaktypeId)
    {
        var history = await getMigrationHistoryService.GetCompletedMigrations(detZaaktypeId);
        return Ok(history);
    }
}
