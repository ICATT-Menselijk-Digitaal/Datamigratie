using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Common.Services.Shared;

namespace Datamigratie.Common.Services.OpenZaak
{
    public abstract class OzPagedApiClient(HttpClient httpClient) : PagedApiClient(httpClient)
    {
        /// <summary>
        /// Gets data from a paginated OpenZaak endpoint.
        /// </summary>
        protected async Task<OzPagedResponse<T>> GetPagedData<T>(string endpoint)
        {
            return await GetPagedData<OzPagedResponse<T>, T>(endpoint);
        }

        /// <summary>
        /// Gets all pages of data from a paginated OpenZaak endpoint.
        /// </summary>
        protected async Task<OzPagedResponse<T>> GetAllPagedData<T>(string initialEndpoint, string? query = null)
        {
            return await GetAllPagedData<OzPagedResponse<T>, T>(
                initialEndpoint,
                query,
                hasNextPage: response => response.NextPage,
                getResults: response => response.Results,
                getCount: response => response.Count,
                createFinalResponse: (count, results) => new OzPagedResponse<T>
                {
                    Count = count,
                    Next = null,
                    Previous = null,
                    Results = results
                });
        }
    }
}

