using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class ResultaatMapper(Dictionary<string, Uri> mappings)
{
    public CreateOzResultaatRequest? Map(DetResultaat detResultaat)
    {
        return mappings.TryGetValue(detResultaat.Naam, out var uri) ? new CreateOzResultaatRequest { Resultaattype = uri, Toelichting = "Resultaat gemigreerd vanuit e-Suite" } : null;
    }
}
