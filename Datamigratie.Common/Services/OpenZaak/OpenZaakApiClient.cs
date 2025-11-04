using System.Net;
using System.Net.Http.Json;
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

        Task<OzZaak> CreateZaak(CreateOzZaakRequest request);

        Task<OzZaak?> GetZaakByIdentificatie(string zaakNummer);

        Task<List<Uri>> GetInformatieobjecttypenUrlsForZaaktype(Uri zaaktypeUri);

        Task<OzDocument> CreateDocument(OzDocument document);

        Task KoppelDocument(OzZaak zaak, OzDocument document, CancellationToken token);

        Task UnlockDocument(OzDocument document, CancellationToken token);

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

            return await response.Content.ReadFromJsonAsync<OzZaaktype>();
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
            var result = await response.Content.ReadFromJsonAsync<OzZaak>()!;
            return result!;
        }

        public async Task<OzDocument> CreateDocument(OzDocument document)
        {
            using var response = await _httpClient.PostAsJsonAsync("documenten/api/v1/enkelvoudiginformatieobjecten", document);
            await response.HandleOpenZaakErrorsAsync();
            var result = await response.Content.ReadFromJsonAsync<OzDocument>();
            return result!;
        }

        public async Task UnlockDocument(OzDocument document, CancellationToken token)
        {
            using var response = await _httpClient.PostAsJsonAsync($"{document.Url}/unlock", new JsonObject { ["lock"] = document.Lock }, cancellationToken: token);
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
            if (document.Bestandsdelen == null || string.IsNullOrWhiteSpace(document.Lock))
                return;

            foreach (var bestandsDeel in document.Bestandsdelen.OrderBy(x => x.Volgnummer))
            {
                var omvang = bestandsDeel.Omvang ?? 0;

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
