using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen
{
    [ApiController]
    [Route("api/oz/zaak")]
    public class ShowZaakController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<OzZaak>> GetZaak()
        {
            return await openZaakApiClient.GetZaak();
        }
    }
}
