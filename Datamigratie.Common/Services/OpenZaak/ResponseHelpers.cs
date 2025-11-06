using System.Net.Http.Json;
using System.Text.Json;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Common.Helpers
{
    public static class ResponseHelpers
    {
        /// <summary>
        /// Ensures that an HTTP response indicates success, and throws a detailed exception if not.
        /// </summary>
        /// <param name="response">
        /// The <see cref="HttpResponseMessage"/> to validate.
        /// </param>
        /// <param name="token">
        /// A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A completed <see cref="Task"/> if the response indicates success (2xx status code).
        /// </returns>
        /// <exception cref="HttpRequestException" />
        public static async Task HandleOpenZaakErrorsAsync(this HttpResponseMessage response, CancellationToken token = default)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            if (response.Content.Headers.ContentType?.MediaType?.Contains("json") != true)
            {
                var str = await response.Content.ReadAsStringAsync(token);
                throw new HttpRequestException(str ?? "Onbekende fout", null, response.StatusCode);
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<OzErrorResponse>(token);

            if (!(errorResponse?.InvalidParams?.Count > 0))
            {
                throw new HttpRequestException(errorResponse?.Detail ?? "Onbekende fout", null, response.StatusCode);
            }

            var invalidFields = string.Join(", ", errorResponse.InvalidParams.Select(p => new { p.Name, p.Reason, p.Code }));
            throw new HttpRequestException($"Validatie fouten: {invalidFields}", null, response.StatusCode);
        }
    }
}
