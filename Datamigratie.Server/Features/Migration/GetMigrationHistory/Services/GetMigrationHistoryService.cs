using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migration.GetMigrationHistory.Models;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.GetMigrationHistory.Services;

public interface IGetMigrationHistoryService
{
    Task<List<MigrationHistoryItem>> GetCompletedMigrations(string detZaaktypeId);
}

public class GetMigrationHistoryService(DatamigratieDbContext context, MigrationWorkerState workerState) : IGetMigrationHistoryService
{
    public async Task<List<MigrationHistoryItem>> GetCompletedMigrations(string detZaaktypeId)
    {
        var query = context.Migrations
            .Where(m => m.DetZaaktypeId == detZaaktypeId);

        // exclude the currently running migration if exists
        if (workerState.IsWorking && workerState.DetZaaktypeId == detZaaktypeId && workerState.MigrationId.HasValue)
        {
            query = query.Where(m => m.Id != workerState.MigrationId.Value);
        }

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MigrationHistoryItem
            {
                Id = m.Id,
                Status = m.Status.ToString(),
                StartedAt = m.StartedAt,
                CompletedAt = m.CompletedAt,
                ErrorMessage = m.ErrorMessage,
                TotalRecords = m.TotalRecords,
                ProcessedRecords = m.ProcessedRecords,
                SuccessfulRecords = m.SuccessfulRecords,
                FailedRecords = m.FailedRecords
            })
            .ToListAsync();
    }
}
