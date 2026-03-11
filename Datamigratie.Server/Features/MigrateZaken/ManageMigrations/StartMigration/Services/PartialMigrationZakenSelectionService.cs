using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services;

public interface IPartialMigrationZakenSelectionService
{
    Task<IReadOnlyList<DetZaakMinimal>> SelectZakenAsync(string detZaaktypeId, CancellationToken ct = default);
}

public class PartialMigrationZakenSelectionService(
    DatamigratieDbContext context,
    IDetApiClient detApiClient) : IPartialMigrationZakenSelectionService
{
    public async Task<IReadOnlyList<DetZaakMinimal>> SelectZakenAsync(string detZaaktypeId, CancellationToken ct = default)
    {
        // Fetch the most recent migration result per zaaknummer in a single query.
        // IsSuccessful = true  → previously succeeded, exclude from re-run
        // IsSuccessful = false → still failed, include in re-run
        var previousMigrationZaken = await context.MigrationRecords
            .Where(r => r.Migration.DetZaaktypeId == detZaaktypeId)
            .GroupBy(r => r.DetZaaknummer)
            .Select(g => new
            {
                DetZaaknummer = g.Key,
                g.OrderByDescending(r => r.ProcessedAt).First().IsSuccessful
            })
            .ToListAsync(ct);

        var stillFailed = previousMigrationZaken
            .Where(r => !r.IsSuccessful)
            .Select(r => new DetZaakMinimal { FunctioneleIdentificatie = r.DetZaaknummer, Open = false })
            .ToList();

        var previouslyAttempted = previousMigrationZaken
            .Select(r => r.DetZaaknummer)
            .ToHashSet();

        var allZakenFromZaaktype = await detApiClient.GetZakenByZaaktype(detZaaktypeId);

        var newlyClosed = allZakenFromZaaktype
            .Where(z => !z.Open && !previouslyAttempted.Contains(z.FunctioneleIdentificatie));

        return [.. stillFailed.Union(newlyClosed)];
    }
}
