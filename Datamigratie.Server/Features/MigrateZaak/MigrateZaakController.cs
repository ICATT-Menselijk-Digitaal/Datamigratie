using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.MigrateZaak
{
    [ApiController]
    [Route("api/migreer-zaak")]
    public class MigrateZaakController : ControllerBase
    {
        private readonly IDetApiClient _detApiClient;
        private readonly IOpenZaakApiClient _openZaakApiClient;
        private readonly ILogger<MigrateZaakController> _logger;
        private readonly IMigrateZaakService _migrateZaakService;


        public MigrateZaakController(
            IDetApiClient detApiClient,
            IOpenZaakApiClient openZaakApiClient,
            ILogger<MigrateZaakController> logger,
            IMigrateZaakService migrateZaakService)
        {
            _detApiClient = detApiClient;
            _openZaakApiClient = openZaakApiClient;
            _logger = logger;
            _migrateZaakService = migrateZaakService;
        }


        /// <summary>
        /// tijdelijk als controller met een Get method geimplementeerd. wordt uiteindelijke een functie die vanuit een mogratie proces aangeroepen wordt
        /// voorbeeld url: http://localhost:56175/api/migreer-zaak?zaaknummer=560-2023&zaaktypeId=3710e7f6-a34b-4cb4-9cb2-561d7d05056b
        /// </summary>
        /// <param name="zaaknummer">functioneleIdentificatie van de zaak in DET. bijvoorbeeld 585-2023</param>
        /// <param name="zaaktypeId">uuid van een zaaktype. bijvoorbeeld 3710e7f6-a34b-4cb4-9cb2-561d7d05056b</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> MigreerZaak([FromQuery] string zaaknummer, [FromQuery] Guid zaaktypeId)
        {

            DetZaak? detZaak;

            try
            {
                detZaak = await _detApiClient.GetZaakByZaaknummer(zaaknummer);
            }
            catch (Exception ex)
            {
                var status = (ex is HttpRequestException httpRequestException && httpRequestException.StatusCode.HasValue)
                    ? (int)httpRequestException.StatusCode
                    : StatusCodes.Status500InternalServerError;

                return Ok(CreateZaakResult.Failed(zaaknummer, "De zaak kon niet opgehaald worden uit het bron systeem.", ex.Message, status));
            }

            try
            {
                var createZaakRequest = _migrateZaakService.CreateOzZaakCreationRequest(detZaak, zaaktypeId);
                var createdZaak = await _openZaakApiClient.CreateZaak(createZaakRequest);
                return Ok(CreateZaakResult.Success(createdZaak.Identificatie, "De zaak is aangemaakt in het doelsysteem"));
            }
            catch (Exception ex)
            {
                var status = (ex is HttpRequestException httpRequestException && httpRequestException.StatusCode.HasValue)
                    ? (int)httpRequestException.StatusCode
                    : StatusCodes.Status500InternalServerError;

                return Ok(CreateZaakResult.Failed(zaaknummer, "De zaak kon niet worden aangemaakt in het doelsysteem.", ex.Message, status));
            }

        }

    }


    public class CreateZaakResult
    {
        public bool IsSuccess { get; private set; }
        public string? Message { get; private set; }
        public string Zaaknummer { get; private set; }
        public string? Details { get; private set; }
        public int? Statuscode { get; private set; }

        private CreateZaakResult(bool isSuccess, string zaaknummer, string? message = null, string? details = null, int? statuscode = null)
        {
            IsSuccess = isSuccess;
            Zaaknummer = zaaknummer;
            Message = message;
            Details = details;
            Statuscode = statuscode;
        }
        public static CreateZaakResult Success(string zaaknummer, string messsage) => new(true, zaaknummer, messsage);
        public static CreateZaakResult Failed(string zaaknummer, string messsage, string details, int? statuscode) => new(false, zaaknummer, messsage, details, statuscode);
    }
}


