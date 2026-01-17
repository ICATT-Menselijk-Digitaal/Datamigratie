using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues.Items
{
    public class MigrationQueueItem
    {
        public required string DetZaaktypeId { get; set; }
        
        /// <summary>
        /// GlobalMapping is validated and set by StartMigrationController before queuing.
        /// It is guaranteed to be non-null and valid when PerformMigrationAsync is called.
        /// </summary>
        public required GlobalMapping GlobalMapping { get; set; }
    }
}
