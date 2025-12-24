using Datamigratie.Server.Features.Mapping.MapResultaattypen.Models;
using Datamigratie.Server.Features.Mapping.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.MapResultaattypen
{
    [ApiController]
    [Route("api/mapping/resultaattype/")]
    public class MapResultaattypenController(IMapResultaattypenService mapResultaattypenService) : ControllerBase
    {
        [HttpPost("{detZaaktypeId}/{detResultaattypeId}")]
        public async Task<ActionResult> PostMapResultaattype(string detZaaktypeId, string detResultaattypeId, [FromBody] ResultaattypeMapping mapping)
        {
            await mapResultaattypenService.CreateResultaattypeMapping(
                detZaaktypeId,
                detResultaattypeId,
                mapping.OzZaaktypeId,
                mapping.OzResultaattypeId);
            return Ok();
        }

        [HttpPut("{detZaaktypeId}/{detResultaattypeId}")]
        public async Task<ActionResult> PutMapResultaattype(string detZaaktypeId, string detResultaattypeId, [FromBody] UpdateResultaattypeMapping mapping)
        {
            await mapResultaattypenService.UpdateResultaattypeMapping(
                detZaaktypeId,
                detResultaattypeId,
                mapping.OzZaaktypeId,
                mapping.UpdatedOzResultaattypeId);
            return Ok();
        }
    }
}
