using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen
{
    [ApiController]
    [Route("api/oz/zaaktypen")]
    public class ShowZaaktypenController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<OzZaaktype>> GetAllZaakTypen()
        {
            return await openZaakApiClient.GetAllZaakTypen();
        }

        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedOzZaaktype>> GetZaaktype(Guid zaaktypeId)
        {
            var ozZaaktype = await openZaakApiClient.GetZaaktype(zaaktypeId);

            if (ozZaaktype == null)
            {
                return NotFound();
            }

            var ozResultaattypen = await openZaakApiClient.GetResultaattypenForZaaktype(zaaktypeId);

            var enrichedOzZaaktype = new EnrichedOzZaaktype
            {
                Url = ozZaaktype.Url,
                Identificatie = ozZaaktype.Identificatie,
                Resultaattypen = ozResultaattypen,
            };

            return enrichedOzZaaktype;

        }
    }
}
