using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class StatusMapper(Dictionary<string, Uri> mappings)
{
    public CreateOzStatusRequest? Map(DetStatus detStatus, DetZaak detZaak, Uri openzaakZaakUri)
    {
        if (!mappings.TryGetValue(detStatus.Naam, out var uri))
        {
            return null;
        }

        if (!detZaak.Einddatum.HasValue)
        {
            throw new InvalidOperationException(
                $"Zaak {detZaak.FunctioneleIdentificatie} has no einddatum. Cannot determine datumStatusGezet.");
        }

        var datumStatusGezet = detZaak.Einddatum.Value.ToDateTime(TimeOnly.MinValue);

        return new CreateOzStatusRequest
        {
            Zaak = openzaakZaakUri,
            Statustype = uri,
            DatumStatusGezet = datumStatusGezet,
            Statustoelichting = "Status gemigreerd vanuit e-Suite"
        };
    }
}
