using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migration.Services;
using Datamigratie.Server.Features.Migration.Workers;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.Services;

public interface IMigrationService
{
    Task StartMigrationAsync();
}

public class MigrationService(MigrationWorker migrationWorker) : IMigrationService
{
    public async Task StartMigrationAsync()
    {
        await migrationWorker.StartAsync(CancellationToken.None);
    }
}
