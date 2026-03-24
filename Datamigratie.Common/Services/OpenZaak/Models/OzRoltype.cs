using Datamigratie.Common.Helpers;

namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class OzRoltype
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
                    Uuid = OzUrlToGuidConverter.ExtractUuidFromUrl(value);
                }
            }
        }

        public Guid Uuid { get; private set; } = Guid.Empty;

        public required string Omschrijving { get; set; }

        public required string Zaaktype { get; set; }
    }
}
