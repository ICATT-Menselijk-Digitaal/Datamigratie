using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Shared.SelectDetZaaktype.ShowDetZaaktypenList
{
    [ApiController]
    [Route("api/det/zaaktypen")]
    public class ShowDetZaaktypenListController(IDetApiClient detApiClient) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<DetZaaktype>> GetAllZaakTypen()
        {
            return await detApiClient.GetAllZaakTypen();
        }
    }
}
