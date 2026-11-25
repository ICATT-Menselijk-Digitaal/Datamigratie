using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping
{
    [ApiController]
    [Route("api/mapping/")]
    public class MapZaaktypenController(IMapZaaktypenService mapZaaktypenService) : ControllerBase
    {

        [HttpPost("{detZaaktypeId}")]
        public async Task<ActionResult> PostMapZaaktypen(string detZaaktypeId, [FromBody] CreateZaaktypenMapping mapping)
        {
            try
            {
                await mapZaaktypenService.CreateZaaktypenMapping(detZaaktypeId, mapping.OzZaaktypeId);
                return Ok();
            }
            catch (ZaaktypeVersiesException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{detZaaktypeId}")]
        public async Task<ActionResult> PutMapZaaktype(string detZaaktypeId, [FromBody] UpdateZaaktypeMapping mapping)
        {
            try
            {
                await mapZaaktypenService.UpdateZaaktypenMapping(detZaaktypeId, mapping.UpdatedOzZaaktypeId);
                return Ok();
            }
            catch (ZaaktypeVersiesException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
