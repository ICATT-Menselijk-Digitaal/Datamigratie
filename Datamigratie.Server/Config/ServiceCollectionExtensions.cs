
using Datamigratie.Common.Extensions;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.ShowMigrationHistory.GetZaaktypeMigrationHistory.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.ShowMigrationHistory.GetZakenMigrationHistory.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Pdf;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documentstatus;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Resultaat;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Status;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PublicatieNiveau;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documenttype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Vertrouwelijkheid;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Besluittype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PdfInformatieobjecttype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Roltype;

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
            services.AddScoped<IBuildMigrationQueueItemService, BuildMigrationQueueItemService>();
            services.AddScoped<IGetMigrationHistoryService, GetMigrationHistoryService>();
            services.AddScoped<IGetMigrationRecordsService, GetMigrationRecordsService>();
            services.AddScoped<IValidateStatusMappingsService, ValidateStatusMappingsService>();
            services.AddScoped<IValidateResultaattypeMappingsService, ValidateResultaattypeMappingsService>();
            services.AddScoped<IValidateDocumentstatusMappingsService, ValidateDocumentstatusMappingsService>();
            services.AddScoped<IValidatePublicatieNiveauMappingsService, ValidatePublicatieNiveauMappingsService>();
            services.AddScoped<IValidateDocumenttypeMappingsService, ValidateDocumenttypeMappingsService>();
            services.AddScoped<IValidateVertrouwelijkheidMappingsService, ValidateVertrouwelijkheidMappingsService>();
            services.AddScoped<IValidateBesluittypeMappingsService, ValidateBesluittypeMappingsService>();
            services.AddScoped<IValidatePdfInformatieobjecttypeMappingService, ValidatePdfInformatieobjecttypeMappingService>();
            services.AddScoped<IValidateRoltypeMappingsService, ValidateRoltypeMappingsService>();

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
