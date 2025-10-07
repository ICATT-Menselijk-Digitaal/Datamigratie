namespace Datamigratie.Server.Features.Migration.Workers
{
    public class MigrationStateService
    {
        private MigrationState _status = MigrationState.Idle;
        private readonly object _lock = new();

        public void SetStatus(MigrationState status)
        {
            lock (_lock) { _status = status; }
        }

        public MigrationState GetStatus()
        {
            lock (_lock) { return _status; }
        }
    }

    public enum MigrationState
    {
        Idle,
        Running,
        Completed,
        Failed,
        Canceled
    }
}
