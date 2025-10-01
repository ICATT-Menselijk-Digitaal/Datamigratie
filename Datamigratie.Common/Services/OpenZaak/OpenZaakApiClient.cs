using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Common.Services.Shared;

namespace Datamigratie.Common.Services.OpenZaak
{
    public interface IOpenZaakApiClient
    {
        Task<List<OzZaaktype>> GetAllZaakTypen();

        Task<OzZaaktype?> GetZaaktype(Guid zaaktypeId);

        Task<OzZaak> CreateZaak(CreateOzZaakRequest request);
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
        public async Task<OzZaaktype?> GetZaaktype(Guid zaaktypeId)
        {
            var endpoint = $"catalogi/api/v1/zaaktypen/{zaaktypeId}";
            var response = await _httpClient.GetAsync(endpoint);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OzZaaktype>(_options);
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

            try
            {
                var content = JsonContent.Create(request);
                content.Headers.Add("Content-Crs", "EPSG:4326");

                var response = await _httpClient.PostAsync(endpoint, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var zaak = await response.Content.ReadFromJsonAsync<OzZaak>(_options);
                    return zaak ?? throw new InvalidOperationException("Failed to deserialize created zaak");
                }

                var errorContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<OzErrorResponse>(errorContent, _options);
                        if (errorResponse?.InvalidParams?.Any() == true)
                        {
                            var invalidFields = string.Join(", ", errorResponse.InvalidParams.Select(p => new { p.Name, p.Reason, p.Code } ));
                            throw new HttpRequestException($"Validation failed for fields: {invalidFields}");
                        }
                        throw new HttpRequestException($"Bad request: {errorResponse?.Detail ?? "Unknown validation error"}");
                    }
                    catch (JsonException)
                    {
                        throw new HttpRequestException($"Bad request: {errorContent}");
                    }
                }

                throw new HttpRequestException($"Failed to create zaak. Status: {response.StatusCode}, Response: {errorContent}");
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Failed to create zaak in OpenZaak", ex);
            }
        }

        protected override int GetDefaultStartingPage()
        {
            return DefaultStartingPage;
        }
    }

    }
