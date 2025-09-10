using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Server.Features.Zaaktypen.ShowZaaktype.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo
{
    [ApiController]
    [Route("api/det/zaaktypen")]
    public class ShowZaaktypeController(IDetApiClient detApiClient, IShowZaaktypeService showZaaktypeService) : ControllerBase
    {

        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedDetZaaktype>> GetZaaktype(string zaaktypeId)
        {
            var zaaktype = await showZaaktypeService.GetZaaktype(zaaktypeId);
            return Ok(zaaktype);
        }

        [HttpGet]
        public async Task<IEnumerable<DetZaaktype>> GetAllZaakTypen()
        {
            return await detApiClient.GetAllZaakTypen();
        }
    }
}
