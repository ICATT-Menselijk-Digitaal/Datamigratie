using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using static Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers.StringTruncationHelper;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class ZaakMapper(string rsin, Uri ozZaaktypeUrl, Dictionary<bool, ZaakVertrouwelijkheidaanduiding> vertrouwelijkheidMappings)
{
    public CreateOzZaakRequest Map(DetZaak detZaak)
    {
        const int MaxZaaknummerLength = 40;
        if (detZaak.FunctioneleIdentificatie.Length > MaxZaaknummerLength)
        {
            throw new InvalidDataException($"Zaak '{detZaak.FunctioneleIdentificatie}' migration failed: The 'zaaknummer' field length ({detZaak.FunctioneleIdentificatie.Length}) exceeds the maximum allowed length of {MaxZaaknummerLength} characters.");
        }

        var registratieDatum = detZaak.CreatieDatumTijd.ToString("yyyy-MM-dd");
        var startDatum = detZaak.Startdatum.ToString("yyyy-MM-dd");

        const int MaxOmschrijvingLength = 80;
        var omschrijving = TruncateWithDots(detZaak.Omschrijving, MaxOmschrijvingLength);

        const int MaxToelichtingLength = 1000;
        var toelichting = TruncateWithDots(detZaak.RedenStart, MaxToelichtingLength);

        var communicatiekanaalNaam = detZaak.Kanaal?.Naam;

        var einddatumGepland = detZaak.Streefdatum.ToString("yyyy-MM-dd");
        var uiterlijkeEinddatumAfdoening = detZaak.Fataledatum?.ToString("yyyy-MM-dd");

        var bewaartermijnEinddatum = detZaak.ArchiveerGegevens?.BewaartermijnEinddatum;

        var overbrengenOp = detZaak.ArchiveerGegevens?.OverbrengenOp;

        if (bewaartermijnEinddatum.HasValue && overbrengenOp.HasValue)
            throw new InvalidDataException($"Zaak '{detZaak.FunctioneleIdentificatie}' bevat zowel 'bewaartermijnEinddatum' als 'overbrengenOp'. Slechts één mag aanwezig zijn.");

        var archiefactiedatum = (bewaartermijnEinddatum ?? overbrengenOp)?.ToString("yyyy-MM-dd");

        var laatsteBetaaldatum = detZaak.Betaalgegevens?.TransactieDatum?.ToString("yyyy-MM-dd");

        List<OzZaakKenmerk>? kenmerken = null;
        if (!string.IsNullOrWhiteSpace(detZaak.ExterneIdentificatie))
        {
            kenmerken =
            [
                new OzZaakKenmerk
                {
                    Kenmerk = detZaak.ExterneIdentificatie,
                    Bron = "e-Suite"
                }
            ];
        }

        OzZaakgeometrie? zaakgeometrie = null;
        if (detZaak.Geolocatie?.Type != null && detZaak.Geolocatie?.Point2D != null)
        {
            zaakgeometrie = new OzZaakgeometrie
            {
                Type = detZaak.Geolocatie.Type,
                Coordinates = detZaak.Geolocatie.Point2D
            };
        }

        var vertrouwelijkheidaanduiding = vertrouwelijkheidMappings.TryGetValue(detZaak.Vertrouwelijk, out var v) ? v
            : throw new InvalidOperationException(
                $"Zaak '{detZaak.FunctioneleIdentificatie}' migration failed: No mapping found for vertrouwelijkheid '{detZaak.Vertrouwelijk}'.");

        return new CreateOzZaakRequest
        {
            Identificatie = detZaak.FunctioneleIdentificatie,
            Bronorganisatie = rsin,
            Omschrijving = omschrijving,
            Zaaktype = ozZaaktypeUrl,
            VerantwoordelijkeOrganisatie = rsin,
            Startdatum = startDatum,
            Registratiedatum = registratieDatum,
            Vertrouwelijkheidaanduiding = vertrouwelijkheidaanduiding,
            Betalingsindicatie = "",
            Archiefstatus = "nog_te_archiveren",
            EinddatumGepland = einddatumGepland,
            UiterlijkeEinddatumAfdoening = uiterlijkeEinddatumAfdoening,
            Toelichting = toelichting ?? "",
            Archiefactiedatum = archiefactiedatum,
            LaatsteBetaaldatum = laatsteBetaaldatum,
            Zaakgeometrie = zaakgeometrie,
            CommunicatiekanaalNaam = communicatiekanaalNaam,
            Kenmerken = kenmerken
        };
    }
}
