using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Server.Features.Mapping.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.MapResultaattypen
{
    [ApiController]
    [Route("api/mapping/resultaattype/")]
    public class MapResultaattypenController(IMapResultaattypenService mapResultaattypenService, IDetApiClient detApiClient) : ControllerBase
    {
        [HttpPost("{detZaaktypeId}/{detResultaattypeId}")]
        public async Task<ActionResult> PostMapResultaattype(string detZaaktypeId, string detResultaattypeId, [FromBody] ResultaattypeMappingRequest mapping)
        {
            var detZaaktype = detApiClient.GetZaaktype(detZaaktypeId);

            if (detZaaktype == null)
            {
                return NotFound($"DET Zaaktype with id {detZaaktypeId} not found");
            }

            await mapResultaattypenService.CreateResultaattypeMapping(
                detZaaktypeId,
                detResultaattypeId,
                mapping.OzResultaattypeId);
            return Ok();
        }

        [HttpPut("{detZaaktypeId}/{detResultaattypeId}")]
        public async Task<ActionResult> PutMapResultaattype(string detZaaktypeId, string detResultaattypeId, [FromBody] ResultaattypeMappingRequest mapping)
        {
            var detZaaktype = detApiClient.GetZaaktype(detZaaktypeId);

            if (detZaaktype == null)
            {
                return NotFound($"DET Zaaktype with id {detZaaktypeId} not found");
            }

            await mapResultaattypenService.UpdateResultaattypeMapping(
                detZaaktypeId,
                detResultaattypeId,
                mapping.OzResultaattypeId);
            return Ok();
        }
    }
}
