using System.Runtime.Serialization;
using Datamigratie.Common.Helpers;

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
                    Id = OzUrlToGuidConverter.ExtractUuidFromUrl(value);
                }
            }
        }

        public required string Identificatie { get; set; }

        public Guid Id { get; private set; } = Guid.Empty;
    }
}
