using Datamigratie.Server.Features.Mapping.MapZaaktypen.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.ShowZaaktypenMapping
{
    [ApiController]
    [Route("api/mapping/")]
    public class ShowZaaktypenMappingController(IShowZaaktypenMappingService showZaaktypenMappingService) : ControllerBase
    {

        [HttpGet("{detZaaktypeId}")]
        public async Task<ActionResult<ZaaktypenMapping>> GetZaaktypenMapping(string detZaaktypeId)
        {
                var mapping = await showZaaktypenMappingService.GetZaaktypenMapping(detZaaktypeId);

                if (mapping == null)
                {
                    return NotFound($"No mapping found for DET Zaaktype ID: {detZaaktypeId}");
                }
                
                return Ok(mapping);
        }
    }
}
