using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo
{
    [ApiController]
    [Route("api/oz/zaaktypen")]
    public class ShowZaaktypenController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<OzZaaktype>> GetAllZaakTypen()
        {
            return await openZaakApiClient.GetAllZaakTypen();
        }
    }
}
