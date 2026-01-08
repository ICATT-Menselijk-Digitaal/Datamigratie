using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypeInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypeInfo
{
    [ApiController]
    [Route("api/oz/zaaktypen")]
    public class ShowOzZaaktypeInfoController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {
        [HttpGet("{zaaktypeUuid}")]
        public async Task<ActionResult<EnrichedOzZaaktype>> GetZaaktype(Guid zaaktypeUuid)
        {
            var ozZaaktype = await openZaakApiClient.GetZaaktype(zaaktypeUuid);

            if (ozZaaktype == null)
            {
                return NotFound();
            }

            var ozStatustypes = await openZaakApiClient.GetStatustypesForZaaktype(new Uri(ozZaaktype.Url));

            var enrichedOzZaaktype = new EnrichedOzZaaktype
            {
                Url = ozZaaktype.Url,
                Identificatie = ozZaaktype.Identificatie,
                Statustypes = ozStatustypes.OrderBy(st => st.Volgnummer).ToList()
            };

            return enrichedOzZaaktype;
        }
    }
}
