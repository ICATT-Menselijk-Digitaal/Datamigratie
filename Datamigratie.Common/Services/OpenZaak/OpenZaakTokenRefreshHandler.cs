using System.Net.Http.Headers;

namespace Datamigratie.Common.Services.OpenZaak
{
    internal class OpenZaakTokenRefreshHandler : DelegatingHandler
    {
        private readonly IOpenZaakTokenProvider _tokenProvider;
        private readonly string _defaultCrs;

        public OpenZaakTokenRefreshHandler(IOpenZaakTokenProvider tokenProvider, string defaultCrs = "EPSG:4326")
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _defaultCrs = defaultCrs;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get a fresh token (will refresh if expired)
            var token = _tokenProvider.GetToken();

            // Update the Authorization header for this request
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Ensure Accept and Accept-Crs headers are set
            if (!request.Headers.Accept.Any())
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            if (!request.Headers.Contains("Accept-Crs"))
            {
                request.Headers.Add("Accept-Crs", _defaultCrs);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
