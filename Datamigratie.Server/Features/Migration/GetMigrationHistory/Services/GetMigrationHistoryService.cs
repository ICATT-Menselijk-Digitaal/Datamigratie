using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migration.GetMigrationHistory.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.GetMigrationHistory.Services;

public interface IGetMigrationHistoryService
{
    Task<List<MigrationHistoryItem>> GetCompletedMigrations(string detZaaktypeId);
}

public class GetMigrationHistoryService(DatamigratieDbContext context) : IGetMigrationHistoryService
{
    public async Task<List<MigrationHistoryItem>> GetCompletedMigrations(string detZaaktypeId)
    {
        var completedStatuses = new[] 
        { 
            MigrationStatus.Completed, 
            MigrationStatus.Failed, 
            MigrationStatus.Cancelled 
        };

        return await context.Migrations
            .Where(m => m.DetZaaktypeId == detZaaktypeId && completedStatuses.Contains(m.Status))
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
