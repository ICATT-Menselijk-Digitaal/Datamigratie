using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.MigrateZaak
{
    [ApiController]
    [Route("api/migreer-zaak")]
    public class MigrateZaakController(
        IDetApiClient detApiClient,
        IMigrateZaakService migrateZaakService) : ControllerBase
    {

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
                detZaak = await detApiClient.GetZaakByZaaknummer(zaaknummer);
            }
            catch (Exception ex)
            {
                var status = (ex is HttpRequestException httpRequestException && httpRequestException.StatusCode.HasValue)
                    ? (int)httpRequestException.StatusCode
                    : StatusCodes.Status500InternalServerError;

                return Ok(MigrateZaakResult.Failed(zaaknummer, "De zaak kon niet opgehaald worden uit het bron systeem.", ex.Message, status));
            }

            var result = await migrateZaakService.MigrateZaak(detZaak, zaaktypeId);
            return Ok(result);

        }

    }
}


