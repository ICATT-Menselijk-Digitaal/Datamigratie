using Datamigratie.Server.Features.Zaaktypen.ShowZaaktype.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.ShowZaaktype
{
    [ApiController]
    [Route("api/oz/zaaktypen")]
    public class ShowZaaktypeController(IShowZaaktypeService showZaaktypeService) : ControllerBase
    {
        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedOzZaaktype>> GetEnrichedZaaktype(Guid zaaktypeId)
        {
            var enrichedOzZaaktype = await showZaaktypeService.GetEnrichedZaaktype(zaaktypeId);

            return enrichedOzZaaktype == null ? NotFound() : enrichedOzZaaktype;
        }
    }
}
