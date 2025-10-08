
using Datamigratie.Common.Extensions;

using Datamigratie.Server.Features.Mapping.MapZaaktypen;
using Datamigratie.Server.Features.Mapping.ShowZaaktypenMapping;
using Datamigratie.Server.Features.Migration.Services;
using Datamigratie.Server.Features.MigrateZaak;
using Datamigratie.Server.Features.Migration.Workers;
using Datamigratie.Server.Helpers;
using Datamigratie.Server.Features.Migration.StartMigration;

namespace Datamigratie.Server.Config
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatamigrationApiClients(configuration);
            services.AddScoped<IMapZaaktypenService, MapZaaktypenService>();
            services.AddScoped<IShowZaaktypenMappingService, ShowZaaktypenMappingService>();
            services.AddScoped<IMigrateZaakService, MigrateZaakService>();

            services.AddScoped<IMigration1Service, Migration1Service>();

            services.AddHostedService<MigrationBackgroundService>();
            services.AddSingleton<IMigrationBackgroundTaskQueue>(ctx =>
            {
                return new MigrationBackgroundTaskQueue(100);
            });
            services.AddSingleton<MigrationWorkerStatus>();
            services.AddScoped<IRandomProvider, RandomProvider>();
            return services;

        }
    }
}
