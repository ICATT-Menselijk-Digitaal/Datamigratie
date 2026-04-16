
using Datamigratie.Common.Extensions;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.SaveResultaattypeMappings;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.ShowResultaattypeMapping;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.SaveStatusMappings.Services;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.ShowStatusMappings.Services;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Services;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.ShowBesluittypeMappings.Services;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.ShowPublicatieNiveauMappings;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.SavePublicatieNiveauMappings;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.ShowDocumenttypeMappings;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings;
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
using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Save.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PublicatieNiveau;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documenttype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Vertrouwelijkheid;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Besluittype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PdfInformatieobjecttype;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Roltype;

namespace Datamigratie.Server.Config
{
    public static class ServiceCollectionExtensions
    {
        private const int MigrationBackgroundTaskQueueCapacity = 1;

        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MigrationOptions>(configuration.GetSection(MigrationOptions.SectionName));
            services.AddDatamigrationApiClients(configuration);
            services.AddScoped<IMapZaaktypenService, MapZaaktypenService>();
            services.AddScoped<IShowZaaktypenMappingService, ShowZaaktypenMappingService>();
            services.AddScoped<ISaveResultaattypenMappingsService, SaveResultaattypenMappingsService>();
            services.AddScoped<IShowResultaattypeMappingService, ShowResultaattypeMappingService>();
            services.AddScoped<IMigrateZaakService, MigrateZaakService>();
            services.AddScoped<IZaakgegevensPdfGenerator, ZaakgegevensPdfGenerator>();

            services.AddScoped<IStartMigrationService, StartMigrationService>();
            services.AddScoped<IBuildMigrationQueueItemService, BuildMigrationQueueItemService>();
            services.AddScoped<IGetMigrationHistoryService, GetMigrationHistoryService>();
            services.AddScoped<IGetMigrationRecordsService, GetMigrationRecordsService>();
            services.AddScoped<IShowStatusMappingsService, ShowStatusMappingsService>();
            services.AddScoped<ISaveStatusMappingsService, SaveStatusMappingsService>();
            services.AddScoped<IShowBesluittypeMappingsService, ShowBesluittypeMappingsService>();
            services.AddScoped<ISaveBesluittypeMappingsService, SaveBesluittypeMappingsService>();
            services.AddScoped<IValidateStatusMappingsService, ValidateStatusMappingsService>();
            services.AddScoped<IValidateResultaattypeMappingsService, ValidateResultaattypeMappingsService>();
            services.AddScoped<IValidateDocumentstatusMappingsService, ValidateDocumentstatusMappingsService>();
            services.AddScoped<ISaveDocumentstatusMappingsService, SaveDocumentstatusMappingsService>();
            services.AddScoped<IValidatePublicatieNiveauMappingsService, ValidatePublicatieNiveauMappingsService>();
            services.AddScoped<IValidateDocumenttypeMappingsService, ValidateDocumenttypeMappingsService>();
            services.AddScoped<IShowPublicatieNiveauMappingsService, ShowPublicatieNiveauMappingsService>();
            services.AddScoped<ISavePublicatieNiveauMappingsService, SavePublicatieNiveauMappingsService>();
            services.AddScoped<IShowDocumenttypeMappingsService, ShowDocumenttypeMappingsService>();
            services.AddScoped<ISaveDocumenttypeMappingsService, SaveDocumenttypeMappingsService>();
            services.AddScoped<IValidateVertrouwelijkheidMappingsService, ValidateVertrouwelijkheidMappingsService>();
            services.AddScoped<IValidateBesluittypeMappingsService, ValidateBesluittypeMappingsService>();
            services.AddScoped<IValidatePdfInformatieobjecttypeMappingService, ValidatePdfInformatieobjecttypeMappingService>();
            services.AddScoped<IShowRoltypeMappingsService, ShowRoltypeMappingsService>();
            services.AddScoped<ISaveRoltypeMappingsService, SaveRoltypeMappingsService>();
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
