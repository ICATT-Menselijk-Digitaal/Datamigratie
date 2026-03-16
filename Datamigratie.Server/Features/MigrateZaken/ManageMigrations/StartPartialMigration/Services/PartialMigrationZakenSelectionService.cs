using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartPartialMigration.Services;

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
        var succesfullmigratedZaken = await context.MigrationRecords
        .Where(r => r.Migration.DetZaaktypeId == detZaaktypeId)
        .GroupBy(r => r.DetZaaknummer)
        .Select(g => new
        {
            DetZaaknummer = g.Key,
            g.OrderByDescending(r => r.ProcessedAt).First().IsSuccessful
        })
        .Where(x => x.IsSuccessful)
        .ToListAsync(ct);

        var allZakenFromZaaktype = await detApiClient.GetZakenByZaaktype(detZaaktypeId);

        var allZakenFromZaaktypeExcludingSuccesfullMigratedZaken = allZakenFromZaaktype.Where(a => succesfullmigratedZaken.All(b => !a.Open && a.FunctioneleIdentificatie != b.DetZaaknummer)).ToList();
        return allZakenFromZaaktypeExcludingSuccesfullMigratedZaken;
    }
}
