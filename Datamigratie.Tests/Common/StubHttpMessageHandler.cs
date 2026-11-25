using System.Net;
using System.Text;

namespace Datamigratie.Tests.Common;

/// <summary>
/// Returns a fixed response for every request and records the requested URIs.
/// </summary>
public class StubHttpMessageHandler(HttpStatusCode statusCode, string? json = null) : HttpMessageHandler
{
    public List<Uri> RequestedUris { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestedUris.Add(request.RequestUri!);
        var response = new HttpResponseMessage(statusCode);
        if (json is not null)
        {
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
        return Task.FromResult(response);
    }
}
