using Datamigratie.Common.Extensions;
using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo;

namespace Datamigratie.Server.Config
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatamigrationApiClients(configuration);
           
            return services;
        }
    }
}
