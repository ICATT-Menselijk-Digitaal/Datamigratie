

namespace Datamigratie.Common.Helpers;


public static class OzUrlToGuidConverter
{

        public static Guid ExtractUuidFromUrl(string url)
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
