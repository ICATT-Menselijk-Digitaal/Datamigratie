namespace Datamigratie.Server.Features.Migration.StartMigration.State
{
    /// <summary>
    /// Thread-safe singleton state for tracking the currently running migration.
    /// Used to prevent concurrent migrations and to exclude running migrations from history queries.
    /// </summary>
    public class MigrationWorkerState
    {
        public volatile bool IsWorking;

        public volatile string? DetZaaktypeId;

        // Note: int? cannot be volatile, but since this is only set/cleared by the background service
        // and read by query services, and we check IsWorking first (which IS volatile),
        // the memory barrier from IsWorking should provide sufficient visibility guarantees.
        public int? MigrationId;
    }
}
