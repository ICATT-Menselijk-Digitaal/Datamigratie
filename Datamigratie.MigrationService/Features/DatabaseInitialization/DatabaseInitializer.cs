using Microsoft.EntityFrameworkCore;
using Datamigratie.Data;

namespace Datamigratie.MigrationService.Features.DatabaseInitialization
{
    public interface IDatabaseInitializer
    {
        Task Initialize(CancellationToken cancellationToken);
    }

    public class DatabaseInitializer(DatamigratieDbContext dbContext) : IDatabaseInitializer
    {
        public Task Initialize(CancellationToken cancellationToken) => RunWithinTransactionAsync(dbContext, async () =>
        {
            Console.WriteLine("init");
            await dbContext.Database.MigrateAsync(cancellationToken);
        }, cancellationToken);

        /// <summary>
        /// Executes operations within a database transaction using EF Core's retrying execution strategy.
        /// This is required for providers like Npgsql that manage retries internally.
        /// </summary>
        private static async Task RunWithinTransactionAsync(DbContext dbContext, Func<Task> handler, CancellationToken cancellationToken)
        {
            Console.WriteLine("r");

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                await handler();
                await transaction.CommitAsync(cancellationToken);
            });
        }
    }
}
