using System.Net.Http.Json;
using Datamigratie.Common.Services.Shared.Models;

namespace Datamigratie.Common.Services.Shared
{
    public abstract class PagedApiClient(HttpClient httpClient)
    {

        protected abstract int GetDefaultStartingPage();

        /// <summary>
        /// Generic method to get data from a paginated endpoint and deserialize it.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the results list.</typeparam>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <returns>A PagedResponse object.</returns>
        protected async Task<PagedResponse<T>> GetPagedData<T>(string endpoint)
        {
            using var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PagedResponse<T>>();
            return result!;
        }

        /// <summary>
        /// Generic method to get all pages of data into one result from a paginated endpoint.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the results list.</typeparam>
        /// <param name="initialEndpoint">The initial API endpoint path (without pagination).
        /// </param>
        /// <returns>A PagedResponse object containing all results across all pages.</returns>
        protected async Task<PagedResponse<T>> GetAllPagedData<T>(string initialEndpoint, string? query = null)
        {
            var allResults = new List<T>();
            var page = GetDefaultStartingPage();
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
    }
}
