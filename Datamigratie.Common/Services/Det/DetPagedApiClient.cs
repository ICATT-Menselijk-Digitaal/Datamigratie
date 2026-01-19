using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.Shared;

namespace Datamigratie.Common.Services.Det
{
    public abstract class DetPagedApiClient(HttpClient httpClient) : PagedApiClient(httpClient)
    {
        /// <summary>
        /// Gets data from a paginated DET endpoint.
        /// </summary>
        protected async Task<DetPagedResponse<T>> GetPagedData<T>(string endpoint)
        {
            return await GetPagedData<DetPagedResponse<T>, T>(endpoint);
        }

        /// <summary>
        /// Gets all pages of data from a paginated DET endpoint.
        /// </summary>
        protected async Task<DetPagedResponse<T>> GetAllPagedData<T>(string initialEndpoint, string? query = null)
        {
            return await GetAllPagedData<DetPagedResponse<T>, T>(
                initialEndpoint,
                query,
                hasNextPage: response => response.NextPage,
                getResults: response => response.Results,
                getCount: response => response.Count,
                createFinalResponse: (count, results) => new DetPagedResponse<T>
                {
                    Count = count,
                    NextPage = false,
                    PreviousPage = false,
                    Results = results
                });
        }
    }
}

