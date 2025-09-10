using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Datamigratie.Common.Authentication;
using Datamigratie.Common.Services.Det;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
                ApplyHeaders(client.DefaultRequestHeaders, "kissdev", openZaakApiKey);
            });
            return services;
        }


        public static void ApplyHeaders(HttpRequestHeaders headers, string clientId, string clientSecret)
        {
            var authHeaderProvider = new ZgwTokenProvider(clientSecret, clientId);
           // var token = authHeaderProvider.GenerateToken();

            const string CLIENT_ID = "kissdev";
            const string SECRET = "GhIj7KlMnOpQrStUvWxYzA1BcDeF2GhIj";
            const string USER_ID = "example@example.com";
            const string USER_REPRESENTATION = "Example Name";

            // Create JWT token
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;

            var tokenDescriptor = new JwtSecurityToken(
                issuer: CLIENT_ID,
                audience: null, // not specified
                claims: new[]
                {
                new Claim("client_id", CLIENT_ID),
                new Claim("user_id", USER_ID),
                new Claim("user_representation", USER_REPRESENTATION)
                },
                notBefore: now,
                expires: now.AddMinutes(10), // Optional expiration
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            string jwtToken = tokenHandler.WriteToken(tokenDescriptor);

            headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            headers.Add(ZgwTokenProvider.CrsHeaderConstants.AcceptCrs, ZgwTokenProvider.CrsHeaderConstants.Value);
            headers.Add(ZgwTokenProvider.CrsHeaderConstants.ContentCrs, ZgwTokenProvider.CrsHeaderConstants.Value);
        }
    }
}
