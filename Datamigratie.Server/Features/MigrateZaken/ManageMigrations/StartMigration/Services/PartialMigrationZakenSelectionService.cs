using Datamigratie.Common.Services.Det;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services;

public interface IPartialMigrationZakenSelectionService
{
    Task<IReadOnlyList<string>> SelectZakenAsync(string detZaaktypeId, CancellationToken ct = default);
}

public class PartialMigrationZakenSelectionService(
    DatamigratieDbContext context,
    IDetApiClient detApiClient) : IPartialMigrationZakenSelectionService
{
    public async Task<IReadOnlyList<string>> SelectZakenAsync(string detZaaktypeId, CancellationToken ct = default)
    {
        var failedZaken = await GetStillFailedZakenAsync(detZaaktypeId, ct);

        var newlyClosedZaken = await GetNewlyClosedZakenAsync(detZaaktypeId, ct);

        var selection = failedZaken.Union(newlyClosedZaken).ToList();

        return selection;
    }

    /// <summary>
    /// Returns zaaknummers whose most recent MigrationRecord (across all runs) is unsuccessful.
    /// Zaken that failed in run 1 but succeeded in run 2 are excluded.
    /// </summary>
    private async Task<HashSet<string>> GetStillFailedZakenAsync(string detZaaktypeId, CancellationToken ct)
    {
        // For each DetZaaknummer that was ever attempted for this zaaktype,
        // get the most recent record and check if it failed.
        var latestRecordPerZaak = await context.MigrationRecords
            .Where(r => r.Migration.DetZaaktypeId == detZaaktypeId)
            .GroupBy(r => r.DetZaaknummer)
            .Select(g => new
            {
                DetZaaknummer = g.Key,
                IsSuccessful = g.OrderByDescending(r => r.ProcessedAt).First().IsSuccessful
            })
            .ToListAsync(ct);

        return latestRecordPerZaak
            .Where(r => !r.IsSuccessful)
            .Select(r => r.DetZaaknummer)
            .ToHashSet();
    }

    /// <summary>
    /// Returns zaaknummers that are currently closed in DET but have never appeared
    /// in any MigrationRecord for this zaaktype (i.e. were open during all previous runs).
    /// </summary>
    private async Task<HashSet<string>> GetNewlyClosedZakenAsync(string detZaaktypeId, CancellationToken ct)
    {
        var previouslyAttempted = await context.MigrationRecords
            .Where(r => r.Migration.DetZaaktypeId == detZaaktypeId)
            .Select(r => r.DetZaaknummer)
            .Distinct()
            .ToHashSetAsync(ct);

        var allCurrentlyClosed = await detApiClient.GetZakenByZaaktype(detZaaktypeId);

        return allCurrentlyClosed
            .Where(z => !z.Open && !previouslyAttempted.Contains(z.FunctioneleIdentificatie))
            .Select(z => z.FunctioneleIdentificatie)
            .ToHashSet();
    }
}
