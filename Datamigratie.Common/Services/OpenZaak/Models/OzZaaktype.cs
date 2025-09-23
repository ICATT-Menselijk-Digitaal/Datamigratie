using System.Runtime.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class OzZaaktype
    {   
        public required string Url { get; set; }

        public string Id { get; private set; } = string.Empty;

        [OnDeserialized]
        public void OnDeserialized()
        {
            Id = ExtractUuidFromUrl(Url);
        }

        private static string ExtractUuidFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty", nameof(url));

            var uuid = url.Split('/').Last();

            // Validate it's actually a valid GUID format
            if (!Guid.TryParse(uuid, out _))
                throw new ArgumentException($"Invalid UUID format in URL: {uuid}");

            return uuid;
        }
    }
}
