using Datamigratie.Server.Features.Mapping.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping
{
    [ApiController]
    [Route("api/mapping/resultaattype/")]
    public class ShowResultaattypeMappingController(IShowResultaattypeMappingService showResultaattypeMappingService) : ControllerBase
    {
        [HttpGet("{detZaaktypeId}/{detResultaattypeId}")]
        public async Task<ActionResult<ResultaattypeMapping>> GetResultaattypeMapping(string detZaaktypeId, string detResultaattypeId)
        {
            var mapping = await showResultaattypeMappingService.GetResultaattypeMapping(detZaaktypeId, detResultaattypeId);

            if (mapping == null)
            {
                return NotFound($"No mapping found for DET Resultaattype ID: {detResultaattypeId} in Zaaktype: {detZaaktypeId}");
            }

            return Ok(mapping);
        }

        [HttpGet("{detZaaktypeId}")]
        public async Task<ActionResult<List<ResultaattypeMapping>>> GetAllResultaattypeMappingsForZaaktype(string detZaaktypeId)
        {
            var mappings = await showResultaattypeMappingService.GetAllResultaattypeMappingsForZaaktype(detZaaktypeId);
            return Ok(mappings);
        }
    }
}
