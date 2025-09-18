using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo;
using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.MapZaaktype
{
    [ApiController]
    [Route("api/mapping/zaaktypen")]
    public class MapZaaktypeController(IShowDetZaaktypeInfoService showZaaktypeService) : ControllerBase
    {

        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedDetZaaktype>> GetZaaktype(string zaaktypeId)
        {
            var zaaktype = await showZaaktypeService.GetZaaktype(zaaktypeId);
            return Ok(zaaktype);
        }
    }
}
