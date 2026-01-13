using Datamigratie.Server.Features.Mapping.ZaaktypeMapping.MapZaaktypen.Models;
using Datamigratie.Server.Features.Mapping.ZaaktypeMapping.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.ZaaktypeMapping.MapZaaktypen
{
    [ApiController]
    [Route("api/mapping/")]
    public class MapZaaktypenController(IMapZaaktypenService mapZaaktypenService) : ControllerBase
    {

        [HttpPost("{detZaaktypeId}")]
        public async Task<ActionResult> PostMapZaaktypen(string detZaaktypeId, [FromBody] CreateZaaktypenMapping mapping)
        {
            await mapZaaktypenService.CreateZaaktypenMapping(detZaaktypeId, mapping.OzZaaktypeId);
            return Ok();
        }

        [HttpPut("{detZaaktypeId}")]
        public async Task<ActionResult> PutMapZaaktype(string detZaaktypeId, [FromBody] UpdateZaaktypeMapping mapping)
        {
            await mapZaaktypenService.UpdateZaaktypenMapping(detZaaktypeId, mapping.UpdatedOzZaaktypeId);
            return Ok();
        }
    }
}
