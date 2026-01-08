using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo
{
    [ApiController]
    [Route("api/det/zaaktypen")]
    public class ShowDetZaaktypeInfoController(IDetApiClient detApiClient) : ControllerBase
    {

        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedDetZaaktype>> GetZaaktype(string zaaktypeId)
        {
            var detZaaktype = await detApiClient.GetZaaktype(zaaktypeId);

            if (detZaaktype == null)
            {
                return NotFound();
            }

            var detZaken = await detApiClient.GetZakenByZaaktype(zaaktypeId);

            var closedDetZaken = detZaken.Count(z => !z.Open);

            var enrichedDetZaaktype = new EnrichedDetZaaktype
            {
                Naam = detZaaktype.Naam,
                Omschrijving = detZaaktype.Omschrijving,
                Actief = detZaaktype.Actief,
                FunctioneleIdentificatie = detZaaktype.FunctioneleIdentificatie,
                Resultaten = detZaaktype.Resultaten,
                ClosedZakenCount = closedDetZaken,
            };

            return enrichedDetZaaktype;

        }
    }
}
