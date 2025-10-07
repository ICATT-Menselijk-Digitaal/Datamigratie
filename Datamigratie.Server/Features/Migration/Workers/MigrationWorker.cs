
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migration.Workers
{
    public class MigrationWorker
    {
        private readonly ILogger<MigrationWorker> _logger;
        private readonly MigrationStateService _stateService;
        private readonly OpenZaakClient _openZaakClient;
        private readonly DatamigratieDbContext _context;

        public MigrationWorker(ILogger<MigrationWorker> logger, MigrationStateService stateService, OpenZaakClient openZaakClient, DatamigratieDbContext context)
        {
            _logger = logger;
            _stateService = stateService;
            _openZaakClient = openZaakClient;
            _context = context; 
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var migration = await _context.Migrations.FirstOrDefaultAsync(m => m.Status == MigrationStatus.Pending);
            
            if (migration == null)
            {
                _logger.LogInformation("No pending migrations found. Migration processing could not be started");
                return;
            }
            migration.StartedAt = DateTime.UtcNow;
            migration.Status = MigrationStatus.InProgress;
        }

    }
}
