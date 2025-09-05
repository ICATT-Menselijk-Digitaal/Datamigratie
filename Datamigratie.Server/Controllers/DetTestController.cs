using Datamigratie.Common.Services.Det;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DetTestController(IDetApiClient _detApiClient) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<Zaaktype>> Get()
        {
            return await _detApiClient.GetZaaktypenAsync();
        }
    }
}
