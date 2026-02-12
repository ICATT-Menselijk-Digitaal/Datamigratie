using Datamigratie.Common.Models;
using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Constants;
using Datamigratie.Server.Features.SelectDetZaaktypeToMigrate.ShowDetZaaktypeInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.SelectDetZaaktypeToMigrate.ShowDetZaaktypeInfo
{
    [ApiController]
    [Route("api/det/zaaktypen")]
    public class ShowDetZaaktypeInfoController(IDetApiClient detApiClient) : ControllerBase
    {

        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedDetZaaktype>> GetZaaktype(string zaaktypeId)
        {
            var detZaaktypeDetail = await detApiClient.GetZaaktypeDetail(zaaktypeId);

            if (detZaaktypeDetail == null)
            {
                return NotFound();
            }

            var detZaken = await detApiClient.GetZakenByZaaktype(zaaktypeId);

            var closedDetZaken = detZaken.Count(z => !z.Open);

            var enrichedDetZaaktype = new EnrichedDetZaaktype
            {
                Naam = detZaaktypeDetail.Naam,
                Omschrijving = detZaaktypeDetail.Omschrijving,
                Actief = detZaaktypeDetail.Actief,
                FunctioneleIdentificatie = detZaaktypeDetail.FunctioneleIdentificatie,
                ClosedZakenCount = closedDetZaken,
                Resultaten = detZaaktypeDetail.Resultaten,
                Statuses = detZaaktypeDetail.Statussen,
                Documenttypen = [.. detZaaktypeDetail.Documenttypen.Select(dt => dt.Documenttype)],
                Besluittypen = [.. detZaaktypeDetail.Besluiten.Select(b => b.Besluittype)],
                PublicatieNiveauOptions = GetPublicatieNiveauOptions(),
                DetVertrouwelijkheidOpties = GetVertrouwelijkheidOpties()
            };

            return enrichedDetZaaktype;
        }

        private static List<ZaaktypeOptionItem> GetPublicatieNiveauOptions()
        {
            return PublicatieNiveauConstants.Values
                .Select(value => new ZaaktypeOptionItem
                {
                    Value = value,
                    Label = FormatPublicatieNiveauLabel(value)
                })
                .ToList();
        }

        private static string FormatPublicatieNiveauLabel(string value)
        {
            return value switch
            {
                "extern" => "Extern",
                "intern" => "Intern",
                "vertrouwelijk" => "Vertrouwelijk",
                _ => value
            };
        }

        private static List<ZaaktypeOptionItem> GetVertrouwelijkheidOpties()
        {
            return
            [
                new ZaaktypeOptionItem { Value = "true", Label = "Ja (Vertrouwelijk)" },
                new ZaaktypeOptionItem { Value = "false", Label = "Nee (Niet vertrouwelijk)" }
            ];
        }
    }
}
