using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Shared.SelectDetZaaktype.ShowDetZaaktypeInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Shared.SelectDetZaaktype.ShowDetZaaktypeInfo
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
                Statuses = detZaaktypeDetail.Statussen
            };

            return enrichedDetZaaktype;

        }
    }
}
