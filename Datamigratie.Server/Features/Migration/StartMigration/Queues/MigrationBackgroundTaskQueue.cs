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
            // Capacity should be set based on the expected application load and
            // number of concurrent threads accessing the queue.            
            // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
            // which completes only when space became available. This leads to backpressure,
            // in case too many publishers/calls start accumulating.
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
