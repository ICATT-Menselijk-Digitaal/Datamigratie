using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration;

public class SingleZaakSelector(string zaaknummer, bool open) : IZakenSelector
{
    public Task<IReadOnlyList<DetZaakMinimal>> SelectZakenAsync(string detZaaktypeId, CancellationToken ct = default)
    {
        IReadOnlyList<DetZaakMinimal> result = [new DetZaakMinimal { FunctioneleIdentificatie = zaaknummer, Open = open }];
        return Task.FromResult(result);
    }
}
