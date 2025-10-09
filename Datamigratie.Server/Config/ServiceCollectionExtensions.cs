﻿
using Datamigratie.Common.Extensions;

using Datamigratie.Server.Features.Mapping.MapZaaktypen;
using Datamigratie.Server.Features.Mapping.ShowZaaktypenMapping;
using Datamigratie.Server.Features.MigrateZaak;
using Datamigratie.Server.Helpers;
using Datamigratie.Server.Features.Migration.StartMigration.Services;
using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.State;

namespace Datamigratie.Server.Config
{
    public static class ServiceCollectionExtensions
    {
        private const int MigrationBackgroundTaskQueueCapacity = 1;

        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatamigrationApiClients(configuration);
            services.AddScoped<IMapZaaktypenService, MapZaaktypenService>();
            services.AddScoped<IShowZaaktypenMappingService, ShowZaaktypenMappingService>();
            services.AddScoped<IMigrateZaakService, MigrateZaakService>();

            services.AddScoped<IStartMigrationService, StartMigrationService>();

            services.AddHostedService<StartMigrationBackgroundService>();
            services.AddSingleton<IMigrationBackgroundTaskQueue>(ctx =>
            {
                return new MigrationBackgroundTaskQueue(MigrationBackgroundTaskQueueCapacity);
            });
            services.AddSingleton<MigrationWorkerStatus>();
            services.AddScoped<IRandomProvider, RandomProvider>();
            return services;

        }
    }
}
