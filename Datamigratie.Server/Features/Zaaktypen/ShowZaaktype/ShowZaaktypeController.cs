using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo.Models;
using Datamigratie.Server.Features.Zaaktypen.ShowZaaktype.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo
{
    [ApiController]
    [Route("api/zaaktype")]
    public class ShowZaaktypeController(IShowZaaktypeService showZaaktypeService) : ControllerBase
    {

        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<Zaaktype>> GetZaaktype([FromBody]ShowZaakTypeRequest zaaktypeRequest, string zaaktypeId)
        {

            var zaaktype = await showZaaktypeService.GetZaaktype(zaaktypeId, zaaktypeRequest.zaaktypeName);

            return Ok(zaaktype);
        }
    }
}
