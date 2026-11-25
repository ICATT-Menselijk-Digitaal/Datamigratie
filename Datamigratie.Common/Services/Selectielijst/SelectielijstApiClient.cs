using System.Net;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using Datamigratie.Common.Services.Selectielijst.Models;

namespace Datamigratie.Common.Services.Selectielijst;

/// <summary>
/// Reads reference data from the Selectielijst API.
/// OpenZaak stores selectielijst references as absolute URLs (zaaktype.selectielijstProcestype and
/// resultaattype.selectielijstklasse), so this client resolves those URLs directly instead of using a configured base URL.
/// </summary>
public interface ISelectielijstApiClient
{
    /// <returns>The procestype, or null if the Selectielijst API does not know the URL</returns>
    Task<SelectielijstProcestype?> GetProcestype(Uri procestypeUrl, CancellationToken token = default);

    /// <returns>The resultaat, or null if the Selectielijst API does not know the URL</returns>
    Task<SelectielijstResultaat?> GetResultaat(Uri resultaatUrl, CancellationToken token = default);
}

public class SelectielijstApiClient(HttpClient httpClient) : ISelectielijstApiClient
{
    public Task<SelectielijstProcestype?> GetProcestype(Uri procestypeUrl, CancellationToken token = default) =>
        GetAsync<SelectielijstProcestype>(procestypeUrl, token);

    public Task<SelectielijstResultaat?> GetResultaat(Uri resultaatUrl, CancellationToken token = default) =>
        GetAsync<SelectielijstResultaat>(resultaatUrl, token);

    private async Task<T?> GetAsync<T>(Uri url, CancellationToken token) where T : class
    {
        if (!url.IsAbsoluteUri)
        {
            throw new ArgumentException($"Selectielijst URL '{url}' must be absolute.", nameof(url));
        }

        using var response = await httpClient.GetAsync(url, token);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(token)
            ?? throw new SerializationException($"Unexpected empty response from the Selectielijst API for '{url}'");
    }
}
