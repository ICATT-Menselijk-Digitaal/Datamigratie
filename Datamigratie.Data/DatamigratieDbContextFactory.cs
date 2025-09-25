using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Datamigratie.Data;

public class DatamigratieDbContextFactory : IDesignTimeDbContextFactory<DatamigratieDbContext>
{
    public DatamigratieDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatamigratieDbContext>();
        
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DatamigratieDb") 
            ?? "Host=localhost;Database=DatamigratieDb;Username=postgres;Password=postgres";
            
        optionsBuilder.UseNpgsql(connectionString);

        return new DatamigratieDbContext(optionsBuilder.Options);
    }
}
