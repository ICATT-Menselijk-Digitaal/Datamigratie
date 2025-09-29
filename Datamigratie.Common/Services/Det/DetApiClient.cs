using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.Shared;
using Datamigratie.Common.Services.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Datamigratie.Common.Services.Det
{
    public interface IDetApiClient
    {
        Task<List<DetZaaktype>> GetAllZaakTypen();

        Task<List<DetZaak>> GetZakenByZaaktype(string zaaktype);

        Task<DetZaak?> GetZaakByZaaknummer(string zaaknummer);

        Task<DetZaaktype?> GetZaaktype(string zaaktypeName);
    }

    public class DetApiClient : PagedApiClient, IDetApiClient
    {
        private const int DefaultStartingPage = 0;

        private readonly HttpClient _httpClient;
        private readonly ILogger<DetApiClient> _logger;

        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public DetApiClient(HttpClient httpClient, ILogger<DetApiClient> logger) : base(httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
        public async Task<DetZaaktype?> GetZaaktype(string zaaktypeName)
        {
            _logger.LogInformation($"Fetching zaaktype with name: {zaaktypeName}");
            var endpoint = $"zaaktypen/{zaaktypeName}";
            var response = await _httpClient.GetAsync(endpoint);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<DetZaaktype>(_options);
        }

        /// <summary>
        /// Gets all zaken for a specific zaaktype, with pagination details.
        /// Endpoint: /zaken?zaaktype={zaaktype}
        /// </summary>
        /// <param name="zaaktype">The type of the zaken to filter by.</param>
        /// <returns>A PagedResponse object containing a list of all Zaak objects across all pages.</returns>
        public async Task<List<DetZaak>> GetZakenByZaaktype(string zaaktype)
        {
            _logger.LogInformation($"Fetching zaken for zaaktype: {zaaktype}");
            var endpoint = $"zaken";
            var query = $"zaaktype={Uri.EscapeDataString(zaaktype)}";
            var pagedZaken = await GetAllPagedData<DetZaak>(endpoint, query);
            return pagedZaken.Results;
        }

        /// <summary>
        /// Gets a specific zaak by its zaaknummer.
        /// Endpoint: /zaken/{zaaknummer}
        /// </summary>
        /// <param name="zaaknummer">The zaaknummer of the zaak to retrieve.</param>
        /// <returns>The DetZaak object if found, otherwise null.</returns>
        public async Task<DetZaak?> GetZaakByZaaknummer(string zaaknummer)
        {
            _logger.LogInformation($"Fetching zaak with zaaknummer: {zaaknummer}");

            try
            {
                var endpoint = $"zaken/{Uri.EscapeDataString(zaaknummer)}";
                var response = await _httpClient.GetAsync(endpoint);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning($"Zaak with zaaknummer {zaaknummer} not found in DET");
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var zaak = await response.Content.ReadFromJsonAsync<DetZaak>(_options);
                _logger.LogInformation($"Successfully retrieved zaak with zaaknummer: {zaaknummer}");
                return zaak;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP error occurred while fetching zaak with zaaknummer: {zaaknummer}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred while fetching zaak with zaaknummer: {zaaknummer}");
                throw;
            }
        }

        protected override int GetDefaultStartingPage()
        {
            return DefaultStartingPage;
        }
    }

    }
