using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo;
using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo
{
    [ApiController]
    [Route("api/det/zaaktypen")]
    public class ShowDetZaaktypeInfoController(IDetApiClient detApiClient) : ControllerBase
    {

        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedDetZaaktype>> GetZaaktype(string zaaktypeId)
        {
            var detZaaktype = await detApiClient.GetSpecificZaaktype(zaaktypeId);

            if (detZaaktype == null)
            {
                return NotFound();
            }

            var detZaken = await detApiClient.GetZakenByZaaktypeAsync(zaaktypeId);

            var closedDetZaken = detZaken.Count(z => !z.Open);

            var enrichedDetZaaktype = new EnrichedDetZaaktype
            {
                Naam = detZaaktype.Naam,
                Omschrijving = detZaaktype.Omschrijving,
                Actief = detZaaktype.Actief,
                FunctioneleIdentificatie = detZaaktype.FunctioneleIdentificatie,
                ClosedZaken = closedDetZaken,
            };

            return enrichedDetZaaktype;

        }
    }
}
