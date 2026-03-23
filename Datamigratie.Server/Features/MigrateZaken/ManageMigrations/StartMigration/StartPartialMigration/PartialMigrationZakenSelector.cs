using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartPartialMigration;

public class PartialMigrationZakenSelector(
    IServiceScopeFactory scopeFactory,
    IDetApiClient detApiClient) : IZakenSelector
{
    public async Task<IReadOnlyList<DetZaakMinimal>> SelectZakenAsync(string detZaaktypeId, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatamigratieDbContext>();

        var successfullyMigratedZaken = await context.MigrationRecords
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

        return allZakenFromZaaktype
            .Where(a => successfullyMigratedZaken.All(b => !a.Open && a.FunctioneleIdentificatie != b.DetZaaknummer))
            .ToList();
    }
}
