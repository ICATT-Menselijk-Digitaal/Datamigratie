using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Server.Features.Zaaktypen.ShowZaaktype.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.ShowZaaktype
{
    [ApiController]
    [Route("api/oz/zaaktypen")]
    public class ShowZaaktypeController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {
        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedOzZaaktype>> GetEnrichedZaaktype(Guid zaaktypeId)
        {
            var ozZaaktype = await openZaakApiClient.GetZaaktype(zaaktypeId);

            if (ozZaaktype == null)
            {
                return NotFound($"OZ Zaaktype is not found with id {zaaktypeId}");
            }

            var ozResultaattypen = await openZaakApiClient.GetResultaattypenForZaaktype(zaaktypeId);

            var enrichedOzZaaktype = new EnrichedOzZaaktype
            {
                Url = ozZaaktype.Url,
                Identificatie = ozZaaktype.Identificatie,
                Resultaattypen = ozResultaattypen,
            };

            return enrichedOzZaaktype == null ? NotFound() : enrichedOzZaaktype;
        }
    }
}
