using Datamigratie.Server.Features.Mapping.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping
{
    [ApiController]
    [Route("api/mapping/resultaattype/")]
    public class ShowResultaattypeMappingController(IShowResultaattypeMappingService showResultaattypeMappingService) : ControllerBase
    {
        [HttpGet("{detZaaktypeId}/all")]
        public async Task<ActionResult<List<ResultaattypeMappingResponse>>> GetAllResultaattypeMappingsForZaaktype(string detZaaktypeId)
        {
            var mappings = await showResultaattypeMappingService.GetAllResultaattypeMappingsForZaaktype(detZaaktypeId);
            return Ok(mappings);
        }
    }
}
