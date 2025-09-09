using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.ShowZaaktypenList
{
    [ApiController]
    [Route("zaaktypen")]
    public class ShowZaaktypenListController(IDetApiClient _detApiClient) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<Zaaktype>> GetAllZaakTypen()
        {
            return await _detApiClient.GetAllZaakTypen();
        }
    }
}
