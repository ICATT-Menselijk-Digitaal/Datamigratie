using Datamigratie.Common.Extensions;
using Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo;

namespace Datamigratie.Server.Config
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatamigrationApiClients(configuration);

            services.AddScoped<IShowZaaktypeService, ShowDetZaaktypeService>();
            return services;
        }
    }
}
