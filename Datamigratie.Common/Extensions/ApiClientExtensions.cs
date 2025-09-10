using Datamigratie.Common.Services.Det;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Datamigratie.Common.Extensions
{
    public static class ApiClientExtensions
    {
        public static IServiceCollection AddDatamigrationApiClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IDetApiClient, DetApiClient>(client =>
            {
                var detApiBaseUrl = configuration.GetValue<string>("DetApi:BaseUrl") ?? throw new Exception("DetApi:BaseUrl configuration value is missing");
                var detApiKey = configuration.GetValue<string>("DetApi:ApiKey") ?? throw new Exception("DetApi:ApiKey configuration value is missing");

                client.BaseAddress = new Uri(detApiBaseUrl);
                client.DefaultRequestHeaders.Add("x-api-key", detApiKey);
            });
            services.AddHttpClient<IOpenZaakApiClient, OpenZaakClient>(client =>
            {
                var openZaakApiBaseUrl = configuration.GetValue<string>("OpenZaakApi:BaseUrl") ?? throw new Exception("OpenZaakApi:BaseUrl configuration value is missing");
                var openZaakApiKey = configuration.GetValue<string>("OpenZaakApi:ApiKey") ?? throw new Exception("OpenZaakApi:ApiKey configuration value is missing");

                client.BaseAddress = new Uri(openZaakApiBaseUrl);
                client.DefaultRequestHeaders.Add("x-api-key", openZaakApiKey);
            });
            return services;
        }
    }
}
