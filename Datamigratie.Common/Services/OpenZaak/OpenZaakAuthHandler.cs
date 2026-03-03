using System.Net;
using System.Net.Http.Headers;
using Datamigratie.Common.Config;
using Microsoft.Extensions.Options;

namespace Datamigratie.Common.Services.OpenZaak;

internal class OpenZaakAuthHandler(IOptions<OpenZaakApiOptions> options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = OpenZaakTokenProvider.GenerateZakenApiToken(options.Value.ApiKey, options.Value.ApiUser);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }
}
