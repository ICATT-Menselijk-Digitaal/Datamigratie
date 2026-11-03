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
        /// <summary>
        /// EF Core 9+ wraps all pending migrations in a single transaction, takes a database lock
        /// and applies the execution strategy itself; wrapping this in a user transaction throws.
        /// </summary>
        public Task Initialize(CancellationToken cancellationToken) =>
            dbContext.Database.MigrateAsync(cancellationToken);
    }
}
