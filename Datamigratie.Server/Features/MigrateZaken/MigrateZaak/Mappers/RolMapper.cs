using System.Diagnostics.CodeAnalysis;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class RolMapper(Dictionary<DetRolType, Uri> roltypeMappings)
{
    public IEnumerable<OzCreateRolRequest> MapRoles(DetZaak detZaak, Uri openZaakZaakUri)
    {
        if (TryMapBehandelaarRol(detZaak, openZaakZaakUri, out var behandelaar))
        {
            yield return behandelaar;
        }

        if (TryMapInitiatorRol(detZaak, openZaakZaakUri, out var initiator))
        {
            yield return initiator;
        }

        foreach (var rol in MapBetrokkeneRollen(detZaak, openZaakZaakUri))
        {
            yield return rol;
        }
    }

    private bool TryMapBehandelaarRol(DetZaak detZaak, Uri openZaakZaakUri, [NotNullWhen(true)] out OzCreateRolRequest? rolRequest)
    {
        rolRequest = null;
        if (string.IsNullOrWhiteSpace(detZaak.Behandelaar) || !roltypeMappings.TryGetValue(DetRolType.behandelaar, out var roltypeUrl))
        {
            return false;
        }
        rolRequest = new OzCreateRolRequest
        {
            Zaak = openZaakZaakUri,
            BetrokkeneType = BetrokkeneType.medewerker,
            Roltype = roltypeUrl,
            BetrokkeneIdentificatie = new OzBetrokkeneIdentificatie
            {
                Identificatie = detZaak.Behandelaar
            }
        };
        return true;
    }

    private bool TryMapInitiatorRol(DetZaak detZaak, Uri openZaakZaakUri, [NotNullWhen(true)] out OzCreateRolRequest? rolRequest)
    {
        rolRequest = null;
        var initiator = detZaak.Initiator;

        if (initiator == null || !roltypeMappings.TryGetValue(DetRolType.initiator, out var roltypeUrl))
        {
            return false;
        }

        var (betrokkeneType, betrokkeneIdentificatie) = MapBetrokkeneIdentificatie(initiator);

        if (HasEmptyBetrokkeneIdentificatie(betrokkeneIdentificatie)) return false;

        rolRequest = new OzCreateRolRequest
        {
            Zaak = openZaakZaakUri,
            Roltype = roltypeUrl,
            BetrokkeneType = betrokkeneType,
            BetrokkeneIdentificatie = betrokkeneIdentificatie
        };

        return true;
    }

    private IEnumerable<OzCreateRolRequest> MapBetrokkeneRollen(DetZaak detZaak, Uri openZaakZaakUri)
    {
        var betrokkenen = detZaak.Betrokkenen;

        if (betrokkenen == null || betrokkenen.Count == 0)
        {
            yield break;
        }

        foreach (var betrokkene in betrokkenen)
        {

            var betrokkeneDetails = betrokkene.Betrokkene;
            var betrokkeneRolType = betrokkene.TypeBetrokkenheid;

            if (!roltypeMappings.TryGetValue(betrokkeneRolType, out var roltypeUrl))
            {
                continue;
            }

            var (betrokkeneType, betrokkeneIdentificatie) = MapBetrokkeneIdentificatie(betrokkeneDetails);
            var rolRequest = new OzCreateRolRequest
            {
                Zaak = openZaakZaakUri,
                Roltype = roltypeUrl,
                BetrokkeneType = betrokkeneType,
                BetrokkeneIdentificatie = betrokkeneIdentificatie,
                IndicatieMachtiging = betrokkeneRolType == DetRolType.gemachtigde
                    ? "gemachtigde"
                    : null
            };

            if (HasEmptyBetrokkeneIdentificatie(rolRequest.BetrokkeneIdentificatie))
            {
                continue;
            }

            yield return rolRequest;
        }
    }

    private static bool HasEmptyBetrokkeneIdentificatie(OzBetrokkeneIdentificatie id) =>
            string.IsNullOrWhiteSpace(id.InpBsn) &&
            string.IsNullOrWhiteSpace(id.KvkNummer) &&
            string.IsNullOrWhiteSpace(id.VestigingsNummer) &&
            string.IsNullOrWhiteSpace(id.Identificatie);

    private static (BetrokkeneType, OzBetrokkeneIdentificatie) MapBetrokkeneIdentificatie(DetSubject subject) =>
        subject switch
        {
            DetPersoon p => (BetrokkeneType.natuurlijk_persoon, new OzBetrokkeneIdentificatie { InpBsn = p.BurgerServiceNummer }),
            DetBedrijf b => (BetrokkeneType.niet_natuurlijk_persoon, new OzBetrokkeneIdentificatie
            {
                KvkNummer = b.KvkNummer,
                VestigingsNummer = b.Vestigingsnummer
            }),
            _ => throw new ArgumentException($"Unknown subject type: {subject.GetType().Name}")
        };
}
