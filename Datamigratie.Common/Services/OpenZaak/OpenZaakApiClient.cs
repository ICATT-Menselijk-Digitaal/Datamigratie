using System.Text.Json;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Common.Services.Shared;
using Microsoft.Extensions.Logging;

namespace Datamigratie.Common.Services.Det
{
    public interface IOpenZaakApiClient
    {
        Task<List<OzZaaktype>> GetAllZaakTypen();

        Task<OzZaaktype> GetZaaktype(Guid zaaktypeId);
    }

    public class OpenZaakClient : PagedApiClient, IOpenZaakApiClient
    {
        private const int DefaultStartingPage = 1;

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public OpenZaakClient(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets all zaaktypen with pagination details.
        /// </summary>
        /// <returns>A PagedResponse object containing a list of all Zaaktype objects across all pages.</returns>
        public async Task<List<OzZaaktype>> GetAllZaakTypen()
        {
            var pagedZaaktypen = await GetAllPagedData<OzZaaktype>("catalogi/api/v1/zaaktypen");
            return pagedZaaktypen.Results;
        }

        /// <summary>
        /// Gets one specific zaaktype by its id.
        /// </summary>
        /// <returns>A zaaktype if found</returns>
        public async Task<OzZaaktype> GetZaaktype(Guid zaaktypeId)
        {
            var endpoint = $"catalogi/api/v1/zaaktypen/{zaaktypeId}";
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OzZaaktype>(jsonString, _options);

            if (result == null)
            {
                throw new Exception($"Zaaktype not found with id {zaaktypeId}");
            }

            return result;
        }

        protected override int GetDefaultStartingPage()
        {
            return DefaultStartingPage;
        }
    }

    }
