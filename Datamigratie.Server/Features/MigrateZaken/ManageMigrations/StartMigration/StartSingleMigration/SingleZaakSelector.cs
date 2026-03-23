using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration;

public class SingleZaakSelector(
    IDetApiClient detApiClient,
    string zaaknummer) : IZakenSelector
{
    public async Task<IReadOnlyList<DetZaakMinimal>> SelectZakenAsync(string detZaaktypeId, CancellationToken ct = default)
    {
        var zaak = await detApiClient.GetZaakByZaaknummer(zaaknummer);
        return [new DetZaakMinimal { FunctioneleIdentificatie = zaaknummer, Open = zaak!.Open }];
    }
}
