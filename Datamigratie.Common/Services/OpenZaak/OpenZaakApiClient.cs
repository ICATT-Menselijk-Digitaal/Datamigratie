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
    }

    public class OpenZaakClient(
        HttpClient httpClient,
        ILogger<OpenZaakClient> logger) : PagedApiClient(httpClient), IOpenZaakApiClient
    {

        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Gets all zaaktypen with pagination details.
        /// Endpoint: /zaaktypen
        /// </summary>
        /// <returns>A PagedResponse object containing a list of all Zaaktype objects across all pages.</returns>
        public async Task<List<OzZaaktype>> GetAllZaakTypen()
        {
            var pagedZaaktypen = await GetAllPagedData<OzZaaktype>("catalogi/api/v1/zaaktypen");
            return pagedZaaktypen.Results;
        }
    }

    }
