using System.Threading.Channels;
using Datamigratie.Server.Features.Migration.StartMigration.Queues.Items;

namespace Datamigratie.Server.Features.Migration.StartMigration.Queues
{
    /// <summary>
    /// Code based on:
    /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-9.0&tabs=visual-studio
    /// </summary>
    public interface IMigrationBackgroundTaskQueue
    {
        ValueTask QueueMigrationAsync(MigrationQueueItem item);

        ValueTask<MigrationQueueItem> DequeueMigrationAsync(
            CancellationToken cancellationToken);
    }

    public class MigrationBackgroundTaskQueue : IMigrationBackgroundTaskQueue
    {
        private readonly Channel<MigrationQueueItem> _queue;

        public MigrationBackgroundTaskQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<MigrationQueueItem>(options);
        }

        public async ValueTask QueueMigrationAsync(
            MigrationQueueItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await _queue.Writer.WriteAsync(item);
        }

        public async ValueTask<MigrationQueueItem> DequeueMigrationAsync(
            CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}
