using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo
{
    [ApiController]
    [Route("zaaktypen")]
    public class ShowZaaktypeInfoController(IDetApiClient _detApiClient) : ControllerBase
    {

        [HttpGet("info/{id}")]
        public async Task<ActionResult<ZaaktypeInfo>> GetZaaktypeInfo(string id)
        {
            var zaken = await _detApiClient.GetZakenByZaaktypeAsync(id);

            return Ok(new ZaaktypeInfo()
            {
                ClosedZaken = zaken.Count(z => !z.Open)
            });
        }
    }
}
