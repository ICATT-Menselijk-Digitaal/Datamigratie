using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaak.CheckZaakExists
{
    [ApiController]
    [Route("api/oz/zaken")]
    public class CheckZaakExistsController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {

        [HttpGet("{zaakNumber}")]
        public async Task<ActionResult<OzZaak>> ZaakExists(string zaakNumber)
        {
            var zaak = await openZaakApiClient.GetZaakByIdentificatie(zaakNumber);

            if (zaak != null)
            {
                return Conflict($"Zaak {zaak.Identificatie} bestaat al in OpenZaak");
            }

            return Ok();
        }
    }
}
