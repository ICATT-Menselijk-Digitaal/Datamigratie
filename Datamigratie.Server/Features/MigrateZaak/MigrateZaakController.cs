 using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Datamigratie.Server.Features.MigrateZaak
{
    [ApiController]
    [Route("api/migreer-zaak")]
    public class MigrateZaakController : ControllerBase
    {
        private readonly IDetApiClient _detApiClient;
        private readonly IOpenZaakApiClient _openZaakApiClient;
        private readonly ILogger<MigrateZaakController> _logger;

        public MigrateZaakController(
            IDetApiClient detApiClient,
            IOpenZaakApiClient openZaakApiClient,
            ILogger<MigrateZaakController> logger)
        {
            _detApiClient = detApiClient;
            _openZaakApiClient = openZaakApiClient;
            _logger = logger;
        }


        /// <summary>
        /// tijdelijk als controller met een Get method geimplementeerd. wordt uiteindelijke een functie die vanuit een mogratie proces aangeroepen wordt
        /// voorbeeld url: http://localhost:56175/api/migreer-zaak?zaaknummer=560-2023&zaaktypeId=https://openzaak.dev.kiss-demo.nl/catalogi/api/v1/zaaktypen/3710e7f6-a34b-4cb4-9cb2-561d7d05056b
        /// </summary>
        /// <param name="zaaknummer">functioneleIdentificatie van de zaak in DET. bijvoorbeeld 585-2023</param>
        /// <param name="zaaktypeId">url van een zaaktype. bijvoorbeeld https://openzaak.dev.kiss-demo.nl/catalogi/api/v1/zaaktypen/3710e7f6-a34b-4cb4-9cb2-561d7d05056b</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetZaakByZaaknummer([FromQuery] string zaaknummer, [FromQuery] string zaaktypeId)
        {

       


            DetZaak? sourceZaak;

            try
            {
                sourceZaak = await _detApiClient.GetZaakByZaaknummer(zaaknummer);

                if (sourceZaak == null)
                {
                    return NotFound($"Zaak {zaaknummer} kon niet gevonden worden in het bron systeem.");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Zaak {Zaaknummer} kon niet worden opgehaald uit de bron.", zaaknummer);

                //if(ex.StatusCode == HttpStatusCode.NotFound)
                //{
                //    return NotFound($"Zaak {zaaknummer} was not found in DET");
                //}

                return ex.StatusCode.HasValue ? StatusCode((int)ex.StatusCode, ex.Message) : StatusCode(500, ex.Message);


                //return ex.StatusCode switch
                //{
                //    HttpStatusCode.NotFound => NotFound(ex.Message),
                //    HttpStatusCode.Forbidden => Unauthorized(ex.Message),
                //    HttpStatusCode.Unauthorized => Unauthorized(ex.Message),
                //    HttpStatusCode.BadGateway => StatusCode((int)(ex.StatusCode ?? 500), ex.Message),
                //    _ => StatusCode(500, ex.Message)
                //};


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving zaak {Zaaknummer}", zaaknummer);
                return StatusCode(500, $"Er is een onverwachte fout opgetreden bij het ophalen van zaak {zaaknummer}: {ex.Message}");
            }

            try
            {

                var createRequest = new CreateOzZaakRequest
                {
                    Identificatie = sourceZaak.FunctioneleIdentificatie,
                    Bronorganisatie = "999990639", // moet een valide rsin zijn
                    Omschrijving = sourceZaak.Omschrijving,
                    Zaaktype = zaaktypeId,
                    VerantwoordelijkeOrganisatie = "999990639",  // moet een valide rsin zijn
                    Startdatum = sourceZaak.Startdatum.ToString("yyyy-MM-dd"),

                    //verplichte velden, ookal zeggen de specs van niet
                    Registratiedatum = sourceZaak.Startdatum.ToString("yyyy-MM-dd"), //todo moet deze zijn, maar die heeft een raar format. moet custom gedeserialized worden sourceZaak.CreatieDatumTijd
                 Vertrouwelijkheidaanduiding = "openbaar", //hier moet in een latere story nog custom mapping voor komen
                 Betalingsindicatie = "",
                 Archiefstatus = "nog_te_archiveren"

                };

                var createdZaak = await _openZaakApiClient.CreateZaak(createRequest);

                return Ok(CreateZaakResult.Success(createdZaak.Identificatie));
            }
            //catch (HttpRequestException ex) when (ex.Message.Contains("DET"))
            //{
            //    _logger.LogError(ex, "DET was not accessible for zaak {Zaaknummer}", zaaknummer);
            //    return CreateZaakResult.Failed("DET was niet bereikbaar");
            //}
            //catch (HttpRequestException ex) when (ex.Message.Contains("Validation failed"))
            //{
            //    _logger.LogError(ex, "OpenZaak validation failed for zaak {Zaaknummer}", zaaknummer);
            //    return CreateZaakResult.Failed($"Zaak van Zaaktype kon niet worden aangemaakt in OZ: {ex.Message}");
            //}
            //catch (HttpRequestException ex)
            //{
            //    _logger.LogError(ex, "OpenZaak was not accessible for zaak {Zaaknummer}", zaaknummer);
            //    return CreateZaakResult.Failed("OZ was niet bereikbaar");
            //}
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating zaak {Zaaknummer} in OpenZaak", zaaknummer);
                return StatusCode(500,  $"Er is een onverwachte fout opgetreden bij het aanmaken van een zaak voor bronzaak {zaaknummer}: {ex.Message}");
            }







            //    if (result.IsSuccess)
            //    {
            //        return Ok(new CreateZaakResponse
            //        {
            //            Success = true,
            //            Message = $"Zaak {zaaknummer} succesvol created in OpenZaak",
            //            ZaakUrl = result.CreatedZaak?.Url
            //        });
            //    }

            //    return BadRequest(new CreateZaakResponse
            //    {
            //        Success = false,
            //        Message = result.ErrorMessage ?? "Unknown error occurred"
            //    });
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unexpected error creating zaak {Zaaknummer} in OpenZaak", zaaknummer);
            //    return StatusCode(500, new CreateZaakResponse
            //    {
            //        Success = false,
            //        Message = "Er is een onverwachte fout opgetreden"
            //    });
            //}











        }


        //            catch (HttpRequestException ex)
        //            {
        //                _logger.LogError(ex, "DET was not accessible for zaak {Zaaknummer}", zaaknummer);
        //                return StatusCode(503, "DET was not accessible");
        //    }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Unexpected error retrieving zaak {Zaaknummer}", zaaknummer);
        //                return StatusCode(500, "An unexpected error occurred");
        //}
        //        }



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
