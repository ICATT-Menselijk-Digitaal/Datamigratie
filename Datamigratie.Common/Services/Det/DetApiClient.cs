using System.Text.Json;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.Extensions.Logging;

namespace Datamigratie.Common.Services.Det
{
    public interface IDetApiClient
    {
        Task<List<DetZaaktype>> GetAllZaakTypen();

        Task<DetZaak> GetSpecificZaakAsync(string zaaktypeId);

        Task<List<DetZaak>> GetZakenByZaaktypeAsync(string zaaktype);

        Task<DetZaaktype?> GetSpecificZaaktype(string zaaktypeName);
    }

    public class DetApiClient(
        HttpClient httpClient,
        ILogger<DetApiClient> logger) : IDetApiClient
    {

        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Generic method to get data from a paginated endpoint and deserialize it.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the results list.</typeparam>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <returns>A PagedResponse object.</returns>
        private async Task<PagedResponse<T>?> GetPagedData<T>(string endpoint)
        {
            var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<PagedResponse<T>>(jsonString, _options);

            return result;
        }

        /// <summary>
        /// Generic method to get all pages of data into one result from a paginated endpoint.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the results list.</typeparam>
        /// <param name="initialEndpoint">The initial API endpoint path (without pagination).
        /// </param>
        /// <returns>A PagedResponse object containing all results across all pages.</returns>
        private async Task<PagedResponse<T>> GetAllPagedData<T>(string initialEndpoint, string? query = null)
        {
            var allResults = new List<T>();
            var page = 0;
            var hasNextPage = true;
            var totalCount = 0;

            while (hasNextPage)
            {
                var endpoint = ConstructPagedEndpoint(initialEndpoint, page, query);
                var pagedResponse = await GetPagedData<T>(endpoint);

                if (pagedResponse == null)
                {
                    break;
                }

                allResults.AddRange(pagedResponse.Results);
                hasNextPage = pagedResponse.NextPage;
                totalCount = pagedResponse.Count;
                page++;
            }

            return new PagedResponse<T>
            {
                Count = totalCount,
                NextPage = false,
                PreviousPage = false,
                Results = allResults
            };
        }

        private static string ConstructPagedEndpoint(string initialEndpoint, int page, string? query = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return $"{initialEndpoint}?page={page}";

            // Trim leading ? or & characters from the query
            var sanitizedQuery = query.TrimStart('?', '&');
            return $"{initialEndpoint}?page={page}&{sanitizedQuery}";
        }

        /// <summary>
        /// Gets all zaaktypen with pagination details.
        /// Endpoint: /zaaktypen
        /// </summary>
        /// <returns>A PagedResponse object containing a list of all Zaaktype objects across all pages.</returns>
        public async Task<List<DetZaaktype>> GetAllZaakTypen()
        {
            var pagedZaaktypen = await GetAllPagedData<DetZaaktype>("zaaktypen");
            return pagedZaaktypen.Results;
        }

        /// <summary>
        /// Gets a specific zaaktype by its name.
        /// Endpoint: /zaaktypen/{name}
        /// </summary>
        /// <returns>A zaaktype object, or null if not found</returns>
        public async Task<DetZaaktype?> GetSpecificZaaktype(string zaaktypeName)
        {
            var endpoint = $"zaaktypen/{zaaktypeName}";
            HttpResponseMessage? response = null;

            try
            {
                response = await httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
            }

            if (response == null)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<DetZaaktype>(jsonString, _options);

            // Despite deserializing to a non nullable type, the outcome can still be null in the odd case when the input is the string "null".
            // We don't consider this an error not just a not found situation.
            if (result == null)
            {
                logger.LogError("Failed to deserialize response from endpoint {endpoint} with response with response {jsonString}", endpoint, jsonString);
                throw new Exception($"Failed to deserialize response from endpoint {endpoint} with response with response {jsonString}");
            }

            return result;
        }

        /// <summary>
        /// Gets a specific zaak by its number.
        /// Endpoint: /zaken/{zaaknummer}
        /// </summary>
        /// <param name="zaaknummer">The number of the specific zaak.</param>
        /// <returns>A Zaak object, or null if not found.</returns>
        public async Task<DetZaak> GetSpecificZaakAsync(string zaaktypeId)
        {
            var endpoint = $"zaken/{zaaktypeId}";
            var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<DetZaak>(jsonString, _options);

            if (result == null)
            {
                logger.LogError("Failed to deserialize response from endpoint {endpoint} with response {jsonString}", endpoint, jsonString);
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
        public async Task<List<DetZaak>> GetZakenByZaaktypeAsync(string zaaktype)
        {
            var endpoint = $"zaken";
            var query = $"zaaktype={Uri.EscapeDataString(zaaktype)}";
            var pagedZaken = await GetAllPagedData<DetZaak>(endpoint, query);
            return pagedZaken.Results;
        }
    }

}
