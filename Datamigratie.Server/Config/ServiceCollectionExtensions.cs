
using Datamigratie.Common.Extensions;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.SaveResultaattypeMappings;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.ShowResultaattypeMapping;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.SaveStatusMappings.Services;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.ShowStatusMappings.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZaaktypeMigrationHistory.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZakenMigrationHistory.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;
using Datamigratie.Server.Features.Migrate.MigrateZaak;
using Datamigratie.Server.Features.Migrate.MigrateZaak.Pdf;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Resultaat;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Status;
using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Save.Services;

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
            services.AddScoped<ISaveResultaattypenMappingsService, SaveResultaattypenMappingsService>();
            services.AddScoped<IShowResultaattypeMappingService, ShowResultaattypeMappingService>();
            services.AddScoped<IMigrateZaakService, MigrateZaakService>();
            services.AddScoped<IZaakgegevensPdfGenerator, ZaakgegevensPdfGenerator>();

            services.AddScoped<IStartMigrationService, StartMigrationService>();
            services.AddScoped<IGetMigrationHistoryService, GetMigrationHistoryService>();
            services.AddScoped<IGetMigrationRecordsService, GetMigrationRecordsService>();
            services.AddScoped<IShowStatusMappingsService, ShowStatusMappingsService>();
            services.AddScoped<ISaveStatusMappingsService, SaveStatusMappingsService>();
            services.AddScoped<IValidateStatusMappingsService, ValidateStatusMappingsService>();
            services.AddScoped<IValidateResultaattypeMappingsService, ValidateResultaattypeMappingsService>();
            services.AddScoped<ISaveDocumentstatusMappingsService, SaveDocumentstatusMappingsService>();

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
