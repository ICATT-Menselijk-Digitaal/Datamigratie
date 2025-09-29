using Datamigratie.Common.Extensions;
using Datamigratie.Server.Features.CreateZaakInOpenZaak.RetrieveDetZaak;
using Datamigratie.Server.Features.Mapping.MapZaaktypen;
using Datamigratie.Server.Features.Mapping.ShowZaaktypenMapping;
using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo;

namespace Datamigratie.Server.Config
{
    public static class ServiceCollectionExtensions
    {
            public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddDatamigrationApiClients(configuration);

            services.AddScoped<IShowDetZaaktypeInfoService, ShowDetZaaktypeInfoService>();
            services.AddScoped<IMapZaaktypenService, MapZaaktypenService>();
            services.AddScoped<IShowZaaktypenMappingService, ShowZaaktypenMappingService>();
            services.AddScoped<IRetrieveDetZaakService, RetrieveDetZaakService>();
            return services;
        }
    }
}
