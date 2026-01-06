namespace Datamigratie.Server.Features.Migration.StartMigration.Queues.Items
{
    public class MigrationQueueItem
    {
        public required string DetZaaktypeId { get; set; }
        
        /// <summary>
        /// GlobalMapping is set by StartMigrationBackgroundService after validation.
        /// It is guaranteed to be non-null when PerformMigrationAsync is called.
        /// </summary>
        public Models.GlobalMapping? GlobalMapping { get; set; }
    }
}
