using System.Net;
using System.Net.Http.Headers;
using Datamigratie.Common.Config;
using Microsoft.Extensions.Options;

namespace Datamigratie.Common.Services.OpenZaak;

internal class OpenZaakAuthHandler(IOptions<OpenZaakApiOptions> options) : DelegatingHandler
{
    // Token lifetime: 1 hour minus 1-min clock leeway; refresh 1m before expiry = 58 min
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromSeconds(55);

    private readonly OpenZaakApiOptions _options = options.Value;
    private string? _cachedToken;
    private DateTime _tokenExpiresAt = DateTime.MinValue;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetOrRefreshToken());

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _tokenExpiresAt = DateTime.MinValue; // force regeneration
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetOrRefreshToken());
            response.Dispose();
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }

    private string GetOrRefreshToken()
    {
        if (_cachedToken == null || DateTime.UtcNow >= _tokenExpiresAt)
        {
            _cachedToken = OpenZaakTokenProvider.GenerateZakenApiToken(_options.ApiKey, _options.ApiUser);
            _tokenExpiresAt = DateTime.UtcNow.Add(TokenLifetime);
        }
        return _cachedToken;
    }
}
