using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartFullMigration;

public class FullMigrationZakenSelector(IDetApiClient detApiClient) : IZakenSelector
{
    public async Task<IReadOnlyList<DetZaakMinimal>> SelectZakenAsync(string detZaaktypeId, CancellationToken ct = default)
    {
        var allZaken = await detApiClient.GetZakenByZaaktype(detZaaktypeId);
        return allZaken.Where(z => !z.Open).ToList();
    }
}
