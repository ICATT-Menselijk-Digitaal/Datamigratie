using System.Net.Http.Headers;
using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.Selectielijst;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Datamigratie.Common.Extensions
{
    public static class ApiClientExtensions
    {
        public static IServiceCollection AddDatamigrationApiClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenZaakApiOptions>(configuration.GetSection("OpenZaakApi"));

            services.AddTransient<OpenZaakAuthHandler>();

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

                client.BaseAddress = new Uri(openZaakApiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept-Crs", "EPSG:4326");
            })
            .AddHttpMessageHandler<OpenZaakAuthHandler>();

            // No base address: OpenZaak hands out absolute Selectielijst URLs, which are public and need no authentication
            services.AddHttpClient<ISelectielijstApiClient, SelectielijstApiClient>(client =>
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            return services;
        }
    }
}
