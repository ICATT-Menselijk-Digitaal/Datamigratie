using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration;

public interface IZakenSelector
{
    Task<IReadOnlyList<DetZaakMinimal>> SelectZakenAsync(string detZaaktypeId, CancellationToken ct = default);
}
