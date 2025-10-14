namespace Datamigratie.Server.Features.Migration.StartMigration.State
{
    public class MigrationWorkerState
    {
        public volatile bool IsWorking;

        public volatile string? DetZaaktypeId;
    }
}
