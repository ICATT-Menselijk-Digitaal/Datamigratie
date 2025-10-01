
using Datamigratie.Server.Features.Mapping.MapZaaktypen;
using Datamigratie.Server.Features.Mapping.ShowZaaktypenMapping;
using Datamigratie.Server.Features.Migration.Services;

namespace Datamigratie.Server.Config
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMapZaaktypenService, MapZaaktypenService>();
            services.AddScoped<IShowZaaktypenMappingService, ShowZaaktypenMappingService>();
            
            services.AddSingleton<MigrationProcessor>();
            services.AddHostedService(sp => sp.GetRequiredService<MigrationProcessor>());
            services.AddSingleton<IMigrationProcessor>(sp => sp.GetRequiredService<MigrationProcessor>());
            
            return services;
        }
    }
}
