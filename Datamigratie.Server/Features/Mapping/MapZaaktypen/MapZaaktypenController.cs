using Datamigratie.Server.Features.Mapping.MapZaaktypen.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.MapZaaktypen
{
    [ApiController]
    [Route("api/mapping/")]
    public class MapZaaktypenController(IMapZaaktypenService mapZaaktypenService) : ControllerBase
    {

        [HttpPost("{detZaaktypeId}")]
        public async Task<ActionResult> PostMapZaaktypen(string detZaaktypeId, [FromBody] CreateZaaktypeMapping mapping)
        {
            try
            {
                await mapZaaktypenService.CreateZaaktypenMapping(detZaaktypeId, mapping.OzZaaktypeId);
                return Ok();

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{detZaaktypeId}")]
        public async Task<ActionResult> PutMapZaaktype(string detZaaktypeId, [FromBody] UpdateZaaktypeMapping mapping)
        {
            try
            {
                await mapZaaktypenService.UpdateZaaktypenMapping(detZaaktypeId, mapping.OzZaaktypeId);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
