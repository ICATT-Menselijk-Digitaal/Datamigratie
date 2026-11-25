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

        public required string Omschrijving { get; set; }

        /// <summary>
        /// URL of the catalogus this zaaktype belongs to. Together with <see cref="Identificatie"/> it identifies
        /// all versions of the same zaaktype.
        /// </summary>
        public required string Catalogus { get; set; }

        /// <summary>
        /// URL of the procestype in the Selectielijst API. The procestype determines the selectielijst year of this version.
        /// </summary>
        public string? SelectielijstProcestype { get; set; }

        public required DateOnly Versiedatum { get; set; }

        public required DateOnly BeginGeldigheid { get; set; }

        public DateOnly? EindeGeldigheid { get; set; }
    }
}
