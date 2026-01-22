using Microsoft.AspNetCore.Mvc;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.Constants;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.GetPublicatieNiveauOptions;

[ApiController]
[Route("api/det/options/publicatieniveau")]
public class GetPublicatieNiveauOptionsController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<string>> GetPublicatieNiveauOptions()
    {
        return Ok(PublicatieNiveauConstants.Values);
    }
}
