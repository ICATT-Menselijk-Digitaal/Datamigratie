using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace Datamigratie.Data;

public static class Extensions
{
    public static IHostApplicationBuilder AddDatamigratieDbContext(this IHostApplicationBuilder builder, string connectionName = "Datamigratie")
    {
        builder.Services.AddDbContext<DatamigratieDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString(connectionName));
        });

        return builder;
    }
}
