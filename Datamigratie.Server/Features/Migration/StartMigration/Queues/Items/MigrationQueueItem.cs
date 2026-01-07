namespace Datamigratie.Server.Features.Migration.StartMigration.Queues.Items
{
    public class MigrationQueueItem
    {
        public required string DetZaaktypeId { get; set; }
        
        /// <summary>
        /// GlobalMapping is validated and set by StartMigrationController before queuing.
        /// It is guaranteed to be non-null and valid when PerformMigrationAsync is called.
        /// </summary>
        public required Models.GlobalMapping GlobalMapping { get; set; }
    }
}
