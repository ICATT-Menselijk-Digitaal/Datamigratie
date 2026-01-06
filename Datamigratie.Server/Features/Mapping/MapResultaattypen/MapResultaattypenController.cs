using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Mapping.Models;
using Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping;
using Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.MapResultaattypen
{
    [ApiController]
    [Route("api/mapping/resultaattype/")]
    public class MapResultaattypenController(IMapResultaattypenService mapResultaattypenService, IShowOzZaaktypenService showZaaktypenService, IDetApiClient detApiClient, IShowResultaattypeMappingService showResultaattypeMappingService) : ControllerBase
    {
        [HttpPost("{detZaaktypeId}")]
        public async Task<ActionResult> PostMapResultaattype(string detZaaktypeId, [FromBody] ResultaattypeMappingRequest mapping)
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

            var resultaattypeMapping = showResultaattypeMappingService.GetResultaattypeMapping(detZaaktypeId);

            if (resultaattypeMapping != null)
            {
                return BadRequest($"ResultaattypeMapping with DET zaaktype id {detZaaktypeId} already exists");
            }

            await mapResultaattypenService.CreateResultaattypeMapping(
                detZaaktypeId,
                mapping.OzZaaktypeId,
                mapping.OzResultaattypeId);
            return Ok();
        }

        [HttpPut("{detZaaktypeId}")]
        public async Task<ActionResult> PutMapResultaattype(string detZaaktypeId, [FromBody] ResultaattypeMappingRequest mapping)
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
                mapping.OzZaaktypeId,
                mapping.OzResultaattypeId);
            return Ok();
        }
    }
}
