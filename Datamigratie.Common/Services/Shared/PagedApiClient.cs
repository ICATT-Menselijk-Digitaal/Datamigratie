using System.Net.Http.Json;
using System.Runtime.Serialization;

namespace Datamigratie.Common.Services.Shared
{
    /// <summary>
    /// Abstract base class for API clients that support pagination.
    /// Subclasses provide specific paged response creation logic.
    /// </summary>
    public abstract class PagedApiClient(HttpClient httpClient)
    {
        protected abstract int GetDefaultStartingPage();

        /// <summary>
        /// Generic method to get data from a paginated endpoint and deserialize it.
        /// </summary>
        /// <typeparam name="TResponse">The type of the paged response.</typeparam>
        /// <typeparam name="T">The type of the objects in the results list.</typeparam>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <returns>A paged response object.</returns>
        protected async Task<TResponse> GetPagedData<TResponse, T>(string endpoint)
        {
            using var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>()
                ?? throw new SerializationException("Unexpected null response");
        }

        /// <summary>
        /// Generic method to get all pages of data into one result from a paginated endpoint.
        /// The subclass must provide a function to extract pagination info and create the final response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the paged response.</typeparam>
        /// <typeparam name="T">The type of the objects in the results list.</typeparam>
        /// <param name="initialEndpoint">The initial API endpoint path (without pagination).</param>
        /// <param name="query">Optional query parameters to append to the endpoint.</param>
        /// <param name="hasNextPage">Function to determine if there's a next page from a response.</param>
        /// <param name="getResults">Function to extract results from a response.</param>
        /// <param name="getCount">Function to extract the total count from a response.</param>
        /// <param name="createFinalResponse">Function to create the final aggregated response.</param>
        /// <returns>A paged response object containing all results across all pages.</returns>
        protected async Task<TResponse> GetAllPagedData<TResponse, T>(
            string initialEndpoint,
            string? query,
            Func<TResponse, bool> hasNextPage,
            Func<TResponse, List<T>> getResults,
            Func<TResponse, int> getCount,
            Func<int, List<T>, TResponse> createFinalResponse)
        {
            var allResults = new List<T>();
            var page = GetDefaultStartingPage();
            var hasNext = true;
            var totalCount = 0;

            while (hasNext)
            {
                var endpoint = ConstructPagedEndpoint(initialEndpoint, page, query);
                var pagedResponse = await GetPagedData<TResponse, T>(endpoint);

                if (pagedResponse == null)
                {
                    break;
                }

                allResults.AddRange(getResults(pagedResponse));
                hasNext = hasNextPage(pagedResponse);
                totalCount = getCount(pagedResponse);
                page++;
            }

            return createFinalResponse(totalCount, allResults);
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
