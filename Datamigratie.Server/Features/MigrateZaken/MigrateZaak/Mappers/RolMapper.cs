using System.Diagnostics.CodeAnalysis;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class RolMapper(Dictionary<DetRolType, Uri> roltypeMappings)
{
    public IEnumerable<OzCreateRolRequest> MapRoles(DetZaak detZaak)
    {
        if (TryMapBehandelaarRol(detZaak, out var behandelaar))
        {
            yield return behandelaar;
        }

        if (TryMapInitiatorRol(detZaak, out var initiator))
        {
            yield return initiator;
        }

        foreach (var rol in MapBetrokkeneRollen(detZaak))
        {
            yield return rol;
        }
    }

    private bool TryMapBehandelaarRol(DetZaak detZaak, [NotNullWhen(true)] out OzCreateRolRequest? rolRequest)
    {
        rolRequest = null;
        if (string.IsNullOrWhiteSpace(detZaak.Behandelaar) || !roltypeMappings.TryGetValue(DetRolType.behandelaar, out var roltypeUrl))
        {
            return false;
        }
        rolRequest = new OzCreateRolRequest
        {
            BetrokkeneType = BetrokkeneType.medewerker,
            Roltype = roltypeUrl,
            BetrokkeneIdentificatie = new OzBetrokkeneIdentificatie
            {
                Identificatie = detZaak.Behandelaar
            }
        };
        return true;
    }

    private bool TryMapInitiatorRol(DetZaak detZaak, [NotNullWhen(true)] out OzCreateRolRequest? rolRequest)
    {
        rolRequest = null;
        var initiator = detZaak.Initiator;

        if (initiator == null || initiator.Subjecttype == null || !roltypeMappings.TryGetValue(DetRolType.initiator, out var roltypeUrl))
        {
            return false;
        }

        var subjecttype = initiator.Subjecttype.Value;

        var (betrokkeneType, betrokkeneIdentificatie) = MapBetrokkeneIdentificatie(subjecttype, initiator);

        if (HasEmptyBetrokkeneIdentificatie(betrokkeneIdentificatie)) return false;

        rolRequest = new OzCreateRolRequest
        {
            Roltype = roltypeUrl,
            BetrokkeneType = betrokkeneType,
            BetrokkeneIdentificatie = betrokkeneIdentificatie
        };

        return true;
    }

    private IEnumerable<OzCreateRolRequest> MapBetrokkeneRollen(DetZaak detZaak)
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

            if (betrokkeneRolType == null ||
                !roltypeMappings.TryGetValue(betrokkeneRolType.Value, out var roltypeUrl) ||
                betrokkeneDetails == null ||
                betrokkeneDetails.Subjecttype == null)
            {
                continue;
            }

            var subjecttype = betrokkeneDetails.Subjecttype;

            var (betrokkeneType, betrokkeneIdentificatie) = MapBetrokkeneIdentificatie(subjecttype.Value, betrokkeneDetails);
            var rolRequest = new OzCreateRolRequest
            {
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

    private static (BetrokkeneType, OzBetrokkeneIdentificatie) MapBetrokkeneIdentificatie(
        DetSubjecttype subjecttype, DetBetrokkenePersoon persoon) =>
        subjecttype == DetSubjecttype.persoon
            ? (BetrokkeneType.natuurlijk_persoon, new OzBetrokkeneIdentificatie { InpBsn = persoon.BurgerServiceNummer })
            : (BetrokkeneType.niet_natuurlijk_persoon, new OzBetrokkeneIdentificatie
            {
                KvkNummer = persoon.KvkNummer,
                VestigingsNummer = persoon.Vestigingsnummer
            });
}
