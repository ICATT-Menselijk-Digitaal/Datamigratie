using System.Text.Json;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.Extensions.Logging;

namespace Datamigratie.Common.Services.Det
{
    public interface IDetApiClient
    {
        Task<List<Zaaktype>> GetAllZaakTypen();

        Task<Zaak> GetSpecificZaakAsync(string zaaktypeId);

        Task<List<Zaak>> GetZakenByZaaktypeAsync(string zaaktype);
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
        private async Task<PagedResponse<T>> GetPagedData<T>(string endpoint)
        {
                var response = await httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
 
                return JsonSerializer.Deserialize<PagedResponse<T>>(jsonString, _options);
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
        public async Task<List<Zaaktype>> GetAllZaakTypen()
        {
            var pagedZaaktypen = await GetAllPagedData<Zaaktype>("zaaktypen");
            return pagedZaaktypen.Results;
        }

        /// <summary>
        /// Gets a specific zaak by its number.
        /// Endpoint: /zaken/{zaaknummer}
        /// </summary>
        /// <param name="zaaknummer">The number of the specific zaak.</param>
        /// <returns>A Zaak object, or null if not found.</returns>
        public async Task<Zaak> GetSpecificZaakAsync(string zaaktypeId)
        {
                var endpoint = $"zaken/{zaaktypeId}";
                var response = await httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Zaak>(jsonString, _options);
        }

        /// <summary>
        /// Gets all zaken for a specific zaaktype, with pagination details.
        /// Endpoint: /zaken?zaaktype={zaaktype}
        /// </summary>
        /// <param name="zaaktype">The type of the zaken to filter by.</param>
        /// <returns>A PagedResponse object containing a list of all Zaak objects across all pages.</returns>
        public async Task<List<Zaak>> GetZakenByZaaktypeAsync(string zaaktype)
        {
            var endpoint = $"zaken";
            var query = $"zaaktype={Uri.EscapeDataString(zaaktype)}";
            var pagedZaken = await GetAllPagedData<Zaak>(endpoint, query);
            return pagedZaken.Results;
        }
    }

    }
