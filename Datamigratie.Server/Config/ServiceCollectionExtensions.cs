using Datamigratie.Common.Extensions;
using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.MapZaaktypen;
using Datamigratie.Server.Features.Mapping.ShowZaaktypenMapping;
using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo;
using Datamigratie.Server.Features.Migration.Services;

namespace Datamigratie.Server.Config
{
    public static class ServiceCollectionExtensions
    {
            public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddDatamigrationApiClients(configuration);
                services.AddScoped<IMapZaaktypenService, MapZaaktypenService>();
                services.AddScoped<IShowDetZaaktypeInfoService, ShowDetZaaktypeInfoService>();
                services.AddScoped<IMigrationProcessor, MigrationProcessor>();

                return services;
            }
    }
}
