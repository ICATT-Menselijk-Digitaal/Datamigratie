using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using static Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers.StringTruncationHelper;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class BesluitMapper(string rsin, Dictionary<string, Uri> besluittypeMappings)
{
    public CreateOzBesluitRequest Map(DetBesluit detBesluit, Uri openZaakZaakUri)
    {
        const int MaxIdentificatieLength = 50;
        var identificatie = TruncateWithDots(detBesluit.FunctioneleIdentificatie, MaxIdentificatieLength);

        var besluittypeUri = besluittypeMappings.TryGetValue(detBesluit.Besluittype.Naam, out var uri) ? uri
            : throw new InvalidOperationException(
                $"Besluit '{detBesluit.FunctioneleIdentificatie}' migration failed: No mapping found for DET besluittype '{detBesluit.Besluittype.Naam}'.");

        var ingangsdatum = detBesluit.Ingangsdatum ?? new DateOnly(1, 1, 1);

        return new CreateOzBesluitRequest
        {
            Zaak = openZaakZaakUri,
            Identificatie = identificatie,
            Besluittype = besluittypeUri,
            VerantwoordelijkeOrganisatie = rsin,
            Datum = detBesluit.BesluitDatum,
            Toelichting = detBesluit.Toelichting ?? "",
            Bestuursorgaan = "",
            Ingangsdatum = ingangsdatum,
            Vervaldatum = detBesluit.Vervaldatum,
            Publicatiedatum = detBesluit.Publicatiedatum,
            UiterlijkeReactiedatum = detBesluit.Reactiedatum,
            Vervalreden = Vervalreden.Blank
        };
    }

}
