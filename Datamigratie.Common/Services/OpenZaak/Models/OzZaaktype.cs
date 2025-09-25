using System.Runtime.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class OzZaaktype
    {

        private string _url = string.Empty;

        public required string Url
        {
            get => _url;
            set
            {
                _url = value;
                if (!string.IsNullOrEmpty(value))
                {
                    Id = ExtractUuidFromUrl(value);
                }
            }
        }

        public required string Identificatie { get; set; }

        public Guid Id { get; private set; } = Guid.Empty;

        private static Guid ExtractUuidFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty", nameof(url));

            var uuid = url.Split('/').Last();

            // Validate it's actually a valid GUID format
            if (!Guid.TryParse(uuid, out _))
                throw new ArgumentException($"Invalid UUID format in URL: {uuid}");

            return Guid.Parse(uuid);
        }
    }
}
