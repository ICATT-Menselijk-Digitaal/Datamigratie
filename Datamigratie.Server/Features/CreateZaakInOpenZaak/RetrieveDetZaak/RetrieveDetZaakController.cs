using Datamigratie.Common.Services.Det.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.CreateZaakInOpenZaak.RetrieveDetZaak
{
    [ApiController]
    [Route("api/create-zaak-in-openzaak/retrieve-det-zaak")]
    public class RetrieveDetZaakController : ControllerBase
    {
        private readonly IRetrieveDetZaakService _retrieveDetZaakService;
        private readonly ILogger<RetrieveDetZaakController> _logger;

        public RetrieveDetZaakController(
            IRetrieveDetZaakService retrieveDetZaakService,
            ILogger<RetrieveDetZaakController> logger)
        {
            _retrieveDetZaakService = retrieveDetZaakService;
            _logger = logger;
        }

        [HttpGet("{zaaknummer}")]
        public async Task<ActionResult<DetZaak>> GetZaakByZaaknummer(string zaaknummer)
        {
            if (string.IsNullOrWhiteSpace(zaaknummer))
            {
                return BadRequest("Zaaknummer is required");
            }

            try
            {
                var zaak = await _retrieveDetZaakService.GetZaakByZaaknummer(zaaknummer);

                if (zaak == null)
                {
                    return NotFound($"Zaak {zaaknummer} was not found in DET");
                }

                return Ok(zaak);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "DET was not accessible for zaak {Zaaknummer}", zaaknummer);
                return StatusCode(503, "DET was not accessible");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving zaak {Zaaknummer}", zaaknummer);
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}
