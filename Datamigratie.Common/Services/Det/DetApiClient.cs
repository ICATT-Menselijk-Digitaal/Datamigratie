using System.Net.Http.Json;
using System.Runtime.Serialization;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.Extensions.Logging;

namespace Datamigratie.Common.Services.Det
{
    public interface IDetApiClient
    {
        Task<List<DetZaaktype>> GetAllZaakTypen();

        Task<List<DetZaakMinimal>> GetZakenByZaaktype(string zaaktype);

        Task<DetZaak?> GetZaakByZaaknummer(string zaaknummer);

        Task<DetZaaktype?> GetZaaktype(string zaaktypeName);

        Task<DetZaaktypeDetail?> GetZaaktypeDetail(string zaaktypeName);

        Task GetDocumentInhoudAsync(long id, Func<Stream, CancellationToken, Task> handleInhoud, CancellationToken token);

        Task<List<DetBesluittype>> GetAllBesluittypen();
    }

    public class DetApiClient(HttpClient httpClient, ILogger<DetApiClient> logger) : DetPagedApiClient(httpClient), IDetApiClient
    {
        private const int DefaultStartingPage = 0;

        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly ILogger<DetApiClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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

                return await response.Content.ReadFromJsonAsync<DetZaaktype>()
                    ?? throw new SerializationException("Unexpected null response");

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while getting zaaktype '{ZaaktypeName}' from endpoint {Endpoint}", SanitizeForLogging(id), SanitizeForLogging(endpoint));
                throw;
            }

        }

        /// <summary>
        /// Gets a specific zaaktype with full details including statuses by its name.
        /// Endpoint: /zaaktypen/{name}
        /// </summary>
        /// <returns>A DetZaaktypeDetail object with statuses, or null if not found</returns>
        public async Task<DetZaaktypeDetail?> GetZaaktypeDetail(string id)
        {
            _logger.LogInformation("Fetching zaaktype detail with statuses for name: {ZaaktypeName}", SanitizeForLogging(id));

            var endpoint = $"zaaktypen/{id}";
            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<DetZaaktypeDetail>()
                    ?? throw new SerializationException("Unexpected null response");

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while getting zaaktype detail '{ZaaktypeName}' from endpoint {Endpoint}", SanitizeForLogging(id), SanitizeForLogging(endpoint));
                throw;
            }

        }

        /// <summary>
        /// Gets all zaken for a specific zaaktype, with pagination details.
        /// Endpoint: /zaken?zaaktype={zaaktype}
        /// </summary>
        /// <param name="zaaktype">The type of the zaken to filter by.</param>
        /// <returns>A PagedResponse object containing a list of all Zaak objects across all pages.</returns>
        public async Task<List<DetZaakMinimal>> GetZakenByZaaktype(string zaaktype)
        {
            _logger.LogInformation("Fetching zaken for zaaktype: {Zaaktype}", SanitizeForLogging(zaaktype));

            var endpoint = $"zaken";
            var query = $"zaaktype={Uri.EscapeDataString(zaaktype)}";
            var pagedZaken = await GetAllPagedData<DetZaakMinimal>(endpoint, query);
            return pagedZaken.Results;
        }

        /// <summary>
        /// Gets a specific zaak by its zaaknummer.
        /// Endpoint: /zaken/{zaaknummer}
        /// </summary>
        /// <param name="zaaknummer">The zaaknummer of the zaak to retrieve. Defined in DET as functioneleIdentificatie</param>
        /// <returns>The DetZaak object if found, otherwise null.</returns>
        public async Task<DetZaak?> GetZaakByZaaknummer(string zaaknummer)
        {
            var endpoint = $"zaken/{Uri.EscapeDataString(zaaknummer)}";
            var response = await _httpClient.GetAsync(endpoint);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<DetZaak>()
                ?? throw new SerializationException("Unexpected null response");
        }

        public async Task GetDocumentInhoudAsync(long id, Func<Stream, CancellationToken, Task> handleInhoud, CancellationToken token)
        {
            var endpoint = $"documenten/inhoud/{id}";
            using var response = await _httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();

            await using var contentStream = await response.Content.ReadAsStreamAsync(token);
            await handleInhoud(contentStream, token);
        }

        /// <summary>
        /// Gets all besluitypen with pagination details.
        /// Endpoint: /besluittypen
        /// </summary>
        /// <returns>A list of all Documenttype objects across all pages.</returns>
        public async Task<List<DetBesluittype>> GetAllBesluittypen()
        {
            _logger.LogInformation("Fetching all besluittypen.");
            var pagedDocumenttypen = await GetAllPagedData<DetBesluittype>("besluittypen");
            return pagedDocumenttypen.Results;
        }

        protected override int GetDefaultStartingPage()
        {
            return DefaultStartingPage;
        }



        private static string SanitizeForLogging(string input) => input.Replace("\r", "").Replace("\n", "");
    }

}
