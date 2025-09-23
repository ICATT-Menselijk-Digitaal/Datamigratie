using Datamigratie.Server.Features.Mapping.MapZaaktypen.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Mapping.ShowZaaktypenMapping
{
    [ApiController]
    [Route("api/mapping/")]
    public class ShowZaaktypenMappingController(IShowZaaktypenMappingService showZaaktypenMappingService) : ControllerBase
    {

        [HttpGet("{detZaaktypeId}")]
        public async Task<ActionResult<ZaaktypenMapping>> GetMapZaaktypen(string detZaaktypeId)
        {
            try
            {
                var mapping = await showZaaktypenMappingService.GetZaaktypenMapping(detZaaktypeId);
                return Ok(mapping);

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
