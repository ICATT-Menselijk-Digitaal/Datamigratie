using System.Net.Http.Headers;
using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Datamigratie.Common.Extensions
{
    public static class ApiClientExtensions
    {
        public static IServiceCollection AddDatamigrationApiClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenZaakApiOptions>(configuration.GetSection("OpenZaakApi"));

            services.AddHttpClient<IDetApiClient, DetApiClient>(client =>
            {
                var detApiBaseUrl = configuration.GetValue<string>("DetApi:BaseUrl") ?? throw new Exception("DetApi:BaseUrl configuration value is missing");
                var detApiKey = configuration.GetValue<string>("DetApi:ApiKey") ?? throw new Exception("DetApi:ApiKey configuration value is missing");

                client.BaseAddress = new Uri(detApiBaseUrl);
                client.DefaultRequestHeaders.Add("x-api-key", detApiKey);
            });

            // Register OpenZaakTokenProvider as singleton
            services.AddSingleton<IOpenZaakTokenProvider>(sp =>
            {
                var openZaakApiKey = configuration.GetValue<string>("OpenZaakApi:ApiKey") ?? throw new Exception("OpenZaakApi:ApiKey configuration value is missing");
                var openZaakApiUser = configuration.GetValue<string>("OpenZaakApi:ApiUser") ?? throw new Exception("OpenZaakApi:ApiUser configuration value is missing");
                return new OpenZaakTokenProvider(openZaakApiKey, openZaakApiUser);
            });

            services.AddHttpClient<IOpenZaakApiClient, OpenZaakClient>(client =>
            {
                var openZaakApiBaseUrl = configuration.GetValue<string>("OpenZaakApi:BaseUrl") ?? throw new Exception("OpenZaakApi:BaseUrl configuration value is missing");
                client.BaseAddress = new Uri(openZaakApiBaseUrl);
            })
            .AddHttpMessageHandler<OpenZaakTokenRefreshHandler>();

            // Register the token refresh handler
            services.AddTransient<OpenZaakTokenRefreshHandler>();

            return services;
        }
    }
}
