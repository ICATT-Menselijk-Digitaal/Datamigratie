using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
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
        /// voorbeeld url: http://localhost:56175/api/migreer-zaak?zaaknummer=560-2023&zaaktypeId=https://openzaak.dev.kiss-demo.nl/catalogi/api/v1/zaaktypen/3710e7f6-a34b-4cb4-9cb2-561d7d05056b
        /// </summary>
        /// <param name="zaaknummer">functioneleIdentificatie van de zaak in DET. bijvoorbeeld 585-2023</param>
        /// <param name="zaaktypeId">url van een zaaktype. bijvoorbeeld https://openzaak.dev.kiss-demo.nl/catalogi/api/v1/zaaktypen/3710e7f6-a34b-4cb4-9cb2-561d7d05056b</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetZaakByZaaknummer([FromQuery] string zaaknummer, [FromQuery] Guid zaaktypeId)
        {

            DetZaak? detZaak;
            OzZaaktype? ozZaakType;

            try
            {

                detZaak = await _detApiClient.GetZaakByZaaknummer(zaaknummer);

                if (detZaak == null)
                {
                    return NotFound($"Zaak {zaaknummer} kon niet gevonden worden in het bron systeem.");
                }
            }
            catch (HttpRequestException ex)
            {
                return ex.StatusCode.HasValue ? StatusCode((int)ex.StatusCode, ex.Message) : StatusCode(500, ex.Message);
            }

            try
            {

                ozZaakType = await _openZaakApiClient.GetZaaktype(zaaktypeId);

                if (ozZaakType == null)
                {
                    return NotFound($"Zaaktype {zaaktypeId} kon niet gevonden worden in het bron systeem");
                }
            }
            catch (HttpRequestException ex)
            {
                return ex.StatusCode.HasValue ? StatusCode((int)ex.StatusCode, ex.Message) : StatusCode(500, ex.Message);
            }

            var createdZaak = await _migrateZaakService.MigrateZaak(detZaak, ozZaakType);

            return Ok(CreateZaakResult.Success(createdZaak.Identificatie));
        }

    }


    public class CreateZaakResult
    {
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string Zaaknummer { get; private set; }

        private CreateZaakResult(bool isSuccess, string zaaknummer, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Zaaknummer = zaaknummer;

        }
        public static CreateZaakResult Success(string zaaknummer) => new(true, zaaknummer);
        public static CreateZaakResult Failed(string errorMessage) => new(false, errorMessage);
    }
}


