using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Datamigratie.Data;

public static class Extensions
{
    public static IHostApplicationBuilder AddDatamigratieDbContext(this IHostApplicationBuilder builder, string connectionName = "Datamigratie")
    {
        builder.AddNpgsqlDbContext<DatamigratieDbContext>(connectionName);
        return builder;
    }
}
