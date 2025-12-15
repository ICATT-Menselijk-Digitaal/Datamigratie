using Datamigratie.Data;
using Datamigratie.Server.Features.Migration.GetMigrationRecords.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.GetMigrationRecords.Services;

public interface IGetMigrationRecordsService
{
    Task<List<MigrationRecordItem>> GetMigrationRecords(int migrationId);
}

public class GetMigrationRecordsService(DatamigratieDbContext context) : IGetMigrationRecordsService
{
    public async Task<List<MigrationRecordItem>> GetMigrationRecords(int migrationId)
    {
        var records = await context.MigrationRecords
            .Where(r => r.MigrationId == migrationId)
            .OrderBy(r => r.DetZaaknummer)
            .Select(r => new MigrationRecordItem
            {
                Id = r.Id,
                DetZaaknummer = r.DetZaaknummer,
                OzZaaknummer = r.OzZaaknummer,
                IsSuccessful = r.IsSuccessful,
                ErrorTitle = r.ErrorTitle,
                ErrorDetails = r.ErrorDetails,
                StatusCode = r.StatusCode,
                ProcessedAt = r.ProcessedAt
            })
            .ToListAsync();

        return records;
    }
}
