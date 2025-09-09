using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DetController(IDetApiClient _detApiClient) : ControllerBase
    {

        [HttpGet]
        [Route("zaaktypen")]
        public async Task<IEnumerable<Zaaktype>> GetAllZaakTypen()
        {
            return await _detApiClient.GetAllZaakTypen();
        }
    }
}
