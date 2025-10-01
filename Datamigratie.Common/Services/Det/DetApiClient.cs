using System.Net.Http.Json;
using System.Text.Json;
using Datamigratie.Common.Extensions;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Common.Services.Shared;
using Datamigratie.Common.Services.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Datamigratie.Common.Services.Det
{
    public interface IDetApiClient
    {
        Task<List<DetZaaktype>> GetAllZaakTypen();

        Task<List<DetZaak>> GetZakenByZaaktype(string zaaktype);

        Task<DetZaak> GetZaak(string zaaktype);

        Task<DetZaaktype?> GetZaaktype(string zaaktypeName);
    }

    public class DetApiClient(HttpClient httpClient, ILogger<DetApiClient> logger) : PagedApiClient(httpClient), IDetApiClient
    {
        private const int DefaultStartingPage = 0;

        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly ILogger<DetApiClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Gets all zaaktypen with pagination details.
        /// Endpoint: /zaaktypen
        /// </summary>
        /// <returns>A PagedResponse object containing a list of all Zaaktype objects across all pages.</returns>
        public async Task<List<DetZaaktype>> GetAllZaakTypen()
        {
            _logger.LogInformation("Fetching all zaaktypen.");
            var pagedZaaktypen = await GetAllPagedData<DetZaaktype>("zaaktypen");
            return pagedZaaktypen.Results;
        }

        /// <summary>
        /// Gets a specific zaaktype by its name.
        /// Endpoint: /zaaktypen/{name}
        /// </summary>
        /// <returns>A zaaktype object, or null if not found</returns>
        public async Task<DetZaaktype?> GetZaaktype(string id)
        {
            _logger.LogInformation("Fetching zaaktype with name: {ZaaktypeName}", SanitizeForLogging(id));

            var endpoint = $"zaaktypen/{id}";
            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<DetZaaktype>(_options);

                // Despite deserializing to a non nullable type, the outcome can still be null in the odd case when the input is the string "null".
                // We don't consider this an error not just a not found situation.
                if (result == null)
                {
                     _logger.LogError("Deserialized response from endpoint {endpoint} is null", SanitizeForLogging(endpoint));
                    throw new Exception($"Deserialized response from endpoint {endpoint} is null");
                }

                return result;

            }
            catch (HttpRequestException ex)
            {
                 _logger.LogError(ex, "An error occurred while getting zaaktype '{ZaaktypeName}' from endpoint {Endpoint}", SanitizeForLogging(id), SanitizeForLogging(endpoint));
                throw;
            }       
 
        }

        /// <summary>
        /// Gets a specific zaak by its number.
        /// Endpoint: /zaken/{zaaknummer}
        /// </summary>
        /// <param name="zaaknummer">The number of the specific zaak.</param>
        /// <returns>A Zaak object, or null if not found.</returns>
        public async Task<DetZaak> GetZaak(string zaaktypeId)
        {
            var endpoint = $"zaken/{zaaktypeId}";
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<DetZaak>(jsonString, _options);

            if (result == null)
            {
                 _logger.LogError("Failed to deserialize response from endpoint {endpoint} with response {jsonString}", endpoint, jsonString);
                throw new Exception($"Failed to deserialize response from endpoint {endpoint} with response {jsonString}");
            }

            return result;
        }

        /// <summary>
        /// Gets all zaken for a specific zaaktype, with pagination details.
        /// Endpoint: /zaken?zaaktype={zaaktype}
        /// </summary>
        /// <param name="zaaktype">The type of the zaken to filter by.</param>
        /// <returns>A PagedResponse object containing a list of all Zaak objects across all pages.</returns>
        public async Task<List<DetZaak>> GetZakenByZaaktype(string zaaktype)
        {
            
            _logger.LogInformation("Fetching zaken for zaaktype: {Zaaktype}", SanitizeForLogging(zaaktype));

            var endpoint = $"zaken";
            var query = $"zaaktype={Uri.EscapeDataString(zaaktype)}";
            var pagedZaken = await GetAllPagedData<DetZaak>(endpoint, query);
            return pagedZaken.Results;
        }

        protected override int GetDefaultStartingPage()
        {
            return DefaultStartingPage;
        }



        private static string SanitizeForLogging(string input) => input.Replace("\r", "").Replace("\n", "");
    }

}
