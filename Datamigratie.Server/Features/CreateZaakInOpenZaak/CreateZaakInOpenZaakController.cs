using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.CreateZaakInOpenZaak
{
    [ApiController]
    [Route("api/create-zaak-in-openzaak")]
    public class CreateZaakInOpenZaakController : ControllerBase
    {
        private readonly ICreateZaakInOpenZaakService _createZaakInOpenZaakService;
        private readonly ILogger<CreateZaakInOpenZaakController> _logger;

        public CreateZaakInOpenZaakController(
            ICreateZaakInOpenZaakService createZaakInOpenZaakService,
            ILogger<CreateZaakInOpenZaakController> logger)
        {
            _createZaakInOpenZaakService = createZaakInOpenZaakService;
            _logger = logger;
        }

        [HttpPost("{zaaknummer}")]
        public async Task<ActionResult<CreateZaakResponse>> CreateZaak(string zaaknummer)
        {
            if (string.IsNullOrWhiteSpace(zaaknummer))
            {
                return BadRequest("Zaaknummer is required");
            }

            try
            {
                var result = await _createZaakInOpenZaakService.CreateZaakInOpenZaak(zaaknummer);

                if (result.IsSuccess)
                {
                    return Ok(new CreateZaakResponse
                    {
                        Success = true,
                        Message = $"Zaak {zaaknummer} succesvol created in OpenZaak",
                        ZaakUrl = result.CreatedZaak?.Url
                    });
                }

                return BadRequest(new CreateZaakResponse
                {
                    Success = false,
                    Message = result.ErrorMessage ?? "Unknown error occurred"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating zaak {Zaaknummer} in OpenZaak", zaaknummer);
                return StatusCode(500, new CreateZaakResponse
                {
                    Success = false,
                    Message = "Er is een onverwachte fout opgetreden"
                });
            }
        }
    }

    public class CreateZaakResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ZaakUrl { get; set; }
    }
}
