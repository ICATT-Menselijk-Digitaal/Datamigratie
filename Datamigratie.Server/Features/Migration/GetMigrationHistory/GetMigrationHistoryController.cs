using Datamigratie.Server.Features.Migration.GetMigrationHistory.Models;
using Datamigratie.Server.Features.Migration.GetMigrationHistory.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Migration.GetMigrationHistory;

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
