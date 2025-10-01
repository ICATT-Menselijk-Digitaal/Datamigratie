using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaak.ShowZaak
{
    [ApiController]
    [Route("api/oz/zaken")]
    public class CheckZaakExistsController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {

        [HttpGet("{zaakId}")]
        public async Task<ActionResult<OzZaak>> ZaakExists(string zaakId)
        {
            var zaak = await openZaakApiClient.GetZaakByIdentificatie(zaakId);

            if (zaak == null)
            {
                return NotFound();
            }

            return Ok(zaak);
        }
    }
}
