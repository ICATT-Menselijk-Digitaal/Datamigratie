using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Mapping.Models;
using Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping;
using Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.MapResultaattypen
{
    [ApiController]
    [Route("api/mapping/resultaattype/")]
    public class MapResultaattypenController(IMapResultaattypenService mapResultaattypenService, IShowZaaktypenService showZaaktypenService, IDetApiClient detApiClient, IShowResultaattypeMappingService showResultaattypeMappingService) : ControllerBase
    {
        [HttpPost("{detZaaktypeId}/{detResultaattypeId}")]
        public async Task<ActionResult> PostMapResultaattype(string detZaaktypeId, string detResultaattypeId, [FromBody] ResultaattypeMappingRequest mapping)
        {
            var detZaaktype = detApiClient.GetZaaktype(detZaaktypeId);

            if (detZaaktype == null)
            {
                return NotFound($"DET Zaaktype with id {detZaaktypeId} not found"); 
            }

            var enrichedOzZaaktype = showZaaktypenService.GetEnrichedZaaktype(mapping.OzZaaktypeId);

            if (enrichedOzZaaktype == null)
            {
                return NotFound($"OZ Zaaktype with id {mapping.OzZaaktypeId} not found");
            }

            var resultaattypeMapping = showResultaattypeMappingService.GetResultaattypeMapping(detZaaktypeId, detResultaattypeId);

            if (resultaattypeMapping != null)
            {
                return BadRequest($"ResultaattypeMapping with DET zaaktype id {detZaaktypeId} and DET resultaattype id {detResultaattypeId} already exists");
            }

            await mapResultaattypenService.CreateResultaattypeMapping(
                detZaaktypeId,
                detResultaattypeId,
                mapping.OzZaaktypeId,
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

            var enrichedOzZaaktype = showZaaktypenService.GetEnrichedZaaktype(mapping.OzZaaktypeId);

            if (enrichedOzZaaktype == null)
            {
                return NotFound($"OZ Zaaktype with id {mapping.OzZaaktypeId} not found");
            }

            await mapResultaattypenService.UpdateResultaattypeMapping(
                detZaaktypeId,
                detResultaattypeId,
                mapping.OzZaaktypeId,
                mapping.OzResultaattypeId);
            return Ok();
        }
    }
}
