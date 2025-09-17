using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo;
using Datamigratie.Server.Features.Zaaktypen.ShowDetZaaktypeInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo
{
    [ApiController]
    [Route("api/det/zaaktypen")]
    public class ShowDetZaaktypeInfoController(IShowDetZaaktypeInfoService showZaaktypeService) : ControllerBase
    {

        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<EnrichedDetZaaktype>> GetZaaktype(string zaaktypeId)
        {
            try
            {
                var zaaktype = await showZaaktypeService.GetZaaktype(zaaktypeId);
                return Ok(zaaktype);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
