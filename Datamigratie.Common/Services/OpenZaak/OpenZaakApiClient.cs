using System.Net;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using Datamigratie.Common.Helpers;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Common.Services.Shared;

namespace Datamigratie.Common.Services.OpenZaak
{
    public interface IOpenZaakApiClient
    {
        Task<List<OzZaaktype>> GetAllZaakTypen();

        Task<OzZaaktype?> GetZaaktype(Guid zaaktypeId);

        Task<List<OzResultaattype>> GetResultaattypenForZaaktype(Guid zaaktypeId);

        Task<List<OzStatustype>> GetStatustypesForZaaktype(Uri zaaktypeUri);

        Task<OzZaak> CreateZaak(CreateOzZaakRequest request);

        Task<OzZaak?> GetZaakByIdentificatie(string zaakNummer);

        Task<List<Uri>> GetInformatieobjecttypenUrlsForZaaktype(Uri zaaktypeUri);

        Task<OzDocument> CreateDocument(OzDocument document);

        Task<OzDocument?> GetDocument(Guid id);

        Task<OzDocument> UpdateDocument(Guid id, OzDocument document);

        Task<string> LockDocument(Guid id, CancellationToken token);

        Task KoppelDocument(OzZaak zaak, OzDocument document, CancellationToken token);

        Task UnlockDocument(Guid id, String? lockToken, CancellationToken token);

        Task UploadBestand(OzDocument document, Stream content, CancellationToken token);
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

        /// <summary>
        /// Gets all statustypes for a specific zaaktype.
        /// </summary>
        /// <returns>A list of statustypes for the zaaktype</returns>
        public async Task<List<OzStatustype>> GetStatustypesForZaaktype(Uri zaaktypeUri)
        {
            var endpoint = $"catalogi/api/v1/statustypen?zaaktype={Uri.EscapeDataString(zaaktypeUri.ToString())}";
            var pagedStatustypes = await GetAllPagedData<OzStatustype>(endpoint);
            return pagedStatustypes.Results;
        }

        /// <summary>
        /// Gets all resultaattypen for a specific zaaktype.
        /// Endpoint: /catalogi/api/v1/resultaattypen?zaaktype={zaaktypeUrl}
        /// </summary>
        /// <param name="zaaktypeId">The ID of the zaaktype</param>
        /// <returns>A list of OzResultaattype objects</returns>
        public async Task<List<OzResultaattype>> GetResultaattypenForZaaktype(Guid zaaktypeId)
        {
            // First get the zaaktype to get its URL
            var zaaktype = await GetZaaktype(zaaktypeId);

            if (zaaktype == null)
            {
                return [];
            }

            var endpoint = "catalogi/api/v1/resultaattypen";
            var query = $"zaaktype={Uri.EscapeDataString(zaaktype.Url)}";

            var pagedResultaattypen = await GetAllPagedData<OzResultaattype>(endpoint, query);

            return pagedResultaattypen.Results;
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

        public async Task<OzDocument> CreateDocument(OzDocument document)
        {
            using var response = await _httpClient.PostAsJsonAsync("documenten/api/v1/enkelvoudiginformatieobjecten", document);
            await response.HandleOpenZaakErrorsAsync();
            return await response.Content.ReadFromJsonAsync<OzDocument>()
                ?? throw new SerializationException("Unexpected null response");
        }

        public async Task<OzDocument> UpdateDocument(Guid documentGuid, OzDocument document)
        {
            var url = $"documenten/api/v1/enkelvoudiginformatieobjecten/{documentGuid}";
            using var response = await _httpClient.PutAsJsonAsync(url, document);
            await response.HandleOpenZaakErrorsAsync();
            return await response.Content.ReadFromJsonAsync<OzDocument>()
                ?? throw new SerializationException("Unexpected null response");
        }

        public async Task<OzDocument?> GetDocument(Guid id)
        {
            var url = $"documenten/api/v1/enkelvoudiginformatieobjecten/{id}";
            using var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            await response.HandleOpenZaakErrorsAsync();
            return await response.Content.ReadFromJsonAsync<OzDocument>()
                ?? throw new SerializationException("Unexpected null response");
        }


        public async Task<string> LockDocument(Guid id, CancellationToken token)
        {
            var url = $"documenten/api/v1/enkelvoudiginformatieobjecten/{id}/lock";
            using var response = await _httpClient.PostAsJsonAsync(url, new JsonObject(), cancellationToken: token);
            await response.HandleOpenZaakErrorsAsync(token: token);
            
            var lockResponse = await response.Content.ReadFromJsonAsync<JsonObject>(cancellationToken: token)
                ?? throw new SerializationException("Unexpected null response from lock endpoint");
            
            return lockResponse["lock"]?.GetValue<string>() 
                ?? throw new SerializationException("Lock token not found in response");
        }

        public async Task UnlockDocument(Guid id, string? lockToken, CancellationToken token)
        {
            var url = $"documenten/api/v1/enkelvoudiginformatieobjecten/{id}/unlock";

            using var response = await _httpClient.PostAsJsonAsync(url, new JsonObject { ["lock"] = lockToken }, cancellationToken: token);
            await response.HandleOpenZaakErrorsAsync(token: token);
        }

        public async Task KoppelDocument(OzZaak zaak, OzDocument document, CancellationToken token)
        {
            using var response = await _httpClient.PostAsJsonAsync("zaken/api/v1/zaakinformatieobjecten", new JsonObject
            {
                ["informatieobject"] = document.Url,
                ["zaak"] = zaak.Url.ToString(),
            }, cancellationToken: token);
            await response.HandleOpenZaakErrorsAsync(token: token);
        }

        public async Task<List<Uri>> GetInformatieobjecttypenUrlsForZaaktype(Uri zaaktypeUri)
        {
            var endpoint = "catalogi/api/v1/zaaktype-informatieobjecttypen";
            var result = await GetAllPagedData<JsonObject>(endpoint, $"zaaktype={zaaktypeUri}");
            return result.Results?.Select(x => x?["informatieobjecttype"]?.GetValue<string>()).OfType<string>().Select(url => new Uri(url)).ToList() ?? [];
        }

        public async Task UploadBestand(OzDocument document, Stream inputStream, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(document.Bestandsdelen);
            ArgumentException.ThrowIfNullOrWhiteSpace(document.Lock);

            foreach (var bestandsDeel in document.Bestandsdelen.OrderBy(x => x.Volgnummer))
            {
                ArgumentNullException.ThrowIfNull(bestandsDeel.Omvang);
                var omvang = bestandsDeel.Omvang.Value;

                using var streamContent = new PushStreamContent(
                    (output) => inputStream.CopyBytesToAsync(output, omvang, token),
                    omvang);

                using var lockContent = new StringContent(document.Lock);

                using var multipart = new MultipartFormDataContent
                {
                    { streamContent, "inhoud", document.Bestandsnaam },
                    { lockContent, "lock" }
                };

                using var response = await _httpClient.PutAsync(bestandsDeel.Url, multipart, token);
                await response.HandleOpenZaakErrorsAsync(token);
            }
        }



        protected override int GetDefaultStartingPage()
        {
            return DefaultStartingPage;
        }

        private class PushStreamContent(Func<Stream, Task> handler, long length = 0) : HttpContent
        {
            private readonly long _length = length;

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) => handler(stream);

            protected override bool TryComputeLength(out long length)
            {
                length = _length;
                return length >= 0;
            }
        }
    }

}
