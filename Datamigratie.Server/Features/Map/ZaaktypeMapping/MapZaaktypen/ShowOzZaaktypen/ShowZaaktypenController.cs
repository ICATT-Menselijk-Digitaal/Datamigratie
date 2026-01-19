using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.MapZaaktypen.ShowOzZaaktypen
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
