using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen
{
    [ApiController]
    [Route("api/oz/zaaktypen")]
    public class ShowZaaktypenController(IShowZaaktypenService showZaaktypenService) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<OzZaaktype>> GetAllZaakTypen()
        {
            return await showZaaktypenService.GetAllZaakTypen();
        }

        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedOzZaaktype>> GetEnrichedZaaktype(Guid zaaktypeId)
        {
            var enrichedOzZaaktype = await showZaaktypenService.GetEnrichedZaaktype(zaaktypeId);

            return enrichedOzZaaktype == null ? NotFound() : enrichedOzZaaktype;
        }
    }
}
