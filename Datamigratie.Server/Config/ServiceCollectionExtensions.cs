
using Datamigratie.Common.Extensions;
using Datamigratie.Server.Features.Mapping.MapResultaattypen;
using Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping;
using Datamigratie.Server.Features.MigrateZaak;
using Datamigratie.Server.Features.MigrateZaak.Pdf;
using Datamigratie.Server.Features.Migration.StartMigration.Services;
using Datamigratie.Server.Features.Migration.StartMigration.Queues;
using Datamigratie.Server.Features.Migration.StartMigration.State;
using Datamigratie.Server.Features.Migration.GetMigrationHistory.Services;
using Datamigratie.Server.Features.Migration.GetMigrationRecords.Services;
using Datamigratie.Server.Features.Mapping.ZaaktypeMapping.ShowZaaktypenMapping;
using Datamigratie.Server.Features.Mapping.ZaaktypeMapping.MapZaaktypen;
using Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen;
using Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Services;
using Datamigratie.Server.Features.Mapping.StatusMapping.SaveStatusMappings.Services;
using Datamigratie.Server.Features.Mapping.StatusMapping.ValidateStatusMappings.Services;

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
            services.AddScoped<IMapResultaattypenService, MapResultaattypenService>();
            services.AddScoped<IShowResultaattypeMappingService, ShowResultaattypeMappingService>();
            services.AddScoped<IMigrateZaakService, MigrateZaakService>();
            services.AddScoped<IZaakgegevensPdfGenerator, ZaakgegevensPdfGenerator>();

            services.AddScoped<IStartMigrationService, StartMigrationService>();
            services.AddScoped<IGetMigrationHistoryService, GetMigrationHistoryService>();
            services.AddScoped<IGetMigrationRecordsService, GetMigrationRecordsService>();
            services.AddScoped<IShowStatusMappingsService, ShowStatusMappingsService>();
            services.AddScoped<ISaveStatusMappingsService, SaveStatusMappingsService>();
            services.AddScoped<IValidateStatusMappingsService, ValidateStatusMappingsService>();

            services.AddHostedService<StartMigrationBackgroundService>();
            services.AddSingleton<IMigrationBackgroundTaskQueue>(ctx =>
            {
                return new MigrationBackgroundTaskQueue(MigrationBackgroundTaskQueueCapacity);
            });
            services.AddSingleton<MigrationWorkerState>();
            
            return services;

        }
    }
}
