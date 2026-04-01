using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Models
{
    public class MigrateZaakMappingModel
    {
        public required ResultaatMapper ResultaatMapper { get; set; }
        public required StatusMapper StatusMapper { get; set; }
        public required ZaakMapper ZaakMapper { get; set; }
        public required DocumentMapper DocumentMapper { get; set; }
        public required BesluitMapper BesluitMapper { get; set; }
        public required PdfMapper PdfMapper { get; set; }
        public required Dictionary<DetRolType, Uri> RoltypeMappings { get; set; }
    }
}
