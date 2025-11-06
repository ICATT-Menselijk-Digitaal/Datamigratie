using System.Net;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using Datamigratie.Common.Helpers;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Common.Services.Shared;

namespace Datamigratie.Common.Services.OpenZaak
{
    public interface IOpenZaakApiClient
    {
        Task<List<OzZaaktype>> GetAllZaakTypen();

        Task<OzZaaktype?> GetZaaktype(Guid zaaktypeId);

        Task<OzZaak> CreateZaak(CreateOzZaakRequest request);

        Task<OzZaak?> GetZaakByIdentificatie(string zaakNummer);

    }

    public class OpenZaakClient : PagedApiClient, IOpenZaakApiClient
    {
        private const int DefaultStartingPage = 1;

        private readonly HttpClient _httpClient;

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
        public async Task<OzZaaktype?> GetZaaktype(Guid zaaktypeId)
        {
            var endpoint = $"catalogi/api/v1/zaaktypen/{zaaktypeId}";
            var response = await _httpClient.GetAsync(endpoint);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            await response.HandleOpenZaakErrorsAsync();

            return await response.Content.ReadFromJsonAsync<OzZaaktype>()
                ?? throw new SerializationException("Unexpected null response");
        }

        public async Task<OzZaak?> GetZaakByIdentificatie(string zaakNummer)
        {
            // uses icontains filter on identificatie field because the search is case-sensitive otherwise
            // currently there is no openzaak filter that allows case-insensitive exact match
            var pagedZaken = await GetAllPagedData<OzZaak>($"zaken/api/v1/zaken", $"identificatie__icontains={zaakNummer}");

            var zaak = pagedZaken.Results.FirstOrDefault(z =>
                string.Equals(z.Identificatie, zaakNummer, StringComparison.OrdinalIgnoreCase)
            );

            return zaak;
        }

        /// <summary>
        /// Creates a new zaak in OpenZaak
        /// </summary>
        /// <param name="request">The zaak creation request</param>
        /// <returns>The created zaak</returns>
        /// <exception cref="HttpRequestException">Thrown when OpenZaak returns validation errors or other HTTP errors</exception>
        public async Task<OzZaak> CreateZaak(CreateOzZaakRequest request)
        {
            var endpoint = "zaken/api/v1/zaken";

            using var content = JsonContent.Create(request);
            content.Headers.Add("Content-Crs", "EPSG:4326");

            using var response = await _httpClient.PostAsync(endpoint, content);
            await response.HandleOpenZaakErrorsAsync();
            return await response.Content.ReadFromJsonAsync<OzZaak>()!
                ?? throw new SerializationException("Unexpected null response"); ;
        }

        protected override int GetDefaultStartingPage()
        {
            return DefaultStartingPage;
        }
    }

}
