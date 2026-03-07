
using Datamigratie.Common.Extensions;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping;
using Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZaaktypeMigrationHistory.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.MigrationHistory.GetZakenMigrationHistory.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;
using Datamigratie.Server.Features.Migrate.MigrateZaak;
using Datamigratie.Server.Features.Migrate.MigrateZaak.Pdf;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documentstatus;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Resultaat;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Status;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.DocumentProperty;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Vertrouwelijkheid;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Besluittype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PdfInformatieobjecttype;

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
            services.AddScoped<IZaakgegevensPdfGenerator, ZaakgegevensPdfGenerator>();

            services.AddScoped<IStartMigrationService, StartMigrationService>();
            services.AddScoped<IGetMigrationHistoryService, GetMigrationHistoryService>();
            services.AddScoped<IGetMigrationRecordsService, GetMigrationRecordsService>();
            services.AddScoped<IValidateStatusMappingsService, ValidateStatusMappingsService>();
            services.AddScoped<IValidateResultaattypeMappingsService, ValidateResultaattypeMappingsService>();
            services.AddScoped<IValidateDocumentstatusMappingsService, ValidateDocumentstatusMappingsService>();
            services.AddScoped<IValidateDocumentPropertyMappingsService, ValidateDocumentPropertyMappingsService>();
            services.AddScoped<IValidateVertrouwelijkheidMappingsService, ValidateVertrouwelijkheidMappingsService>();
            services.AddScoped<IValidateBesluittypeMappingsService, ValidateBesluittypeMappingsService>();
            services.AddScoped<IValidatePdfInformatieobjecttypeMappingService, ValidatePdfInformatieobjecttypeMappingService>();

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
