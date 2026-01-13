using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping
{
    [ApiController]
    [Route("api/mappings/{zaaktypenMappingId:guid}/resultaattypen")]
    public class ShowResultaattypeMappingController(IShowResultaattypeMappingService showResultaattypeMappingService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ResultaattypeMappingResponse>>> GetResultaattypenMappings([FromRoute] Guid zaaktypenMappingId)
        {
            var mappings = await showResultaattypeMappingService.GetResultaattypenMappings(zaaktypenMappingId);
            return Ok(mappings);
        }
    }
}
