using Datamigratie.Common.Helpers;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowOzZaaktypen.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowOzZaaktypen
{
    [ApiController]
    [Route("api/oz/zaaktypen")]
    public class ShowZaaktypenController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {
        /// <summary>
        /// Lists every published OZ zaaktype once, represented by its newest version, instead of every version separately.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<OzZaaktypeOptie>> GetAllZaakTypen()
        {
            var zaaktypen = await openZaakApiClient.GetAllZaakTypen();

            return zaaktypen
                .GroupBy(z => (z.Identificatie, z.Catalogus))
                // Published versions of a zaaktype never overlap in validity, so the latest start is the newest version
                .Select(versies => versies.MaxBy(z => z.BeginGeldigheid)!)
                .Select(newest => new OzZaaktypeOptie
                {
                    Id = newest.Id,
                    Identificatie = newest.Identificatie,
                    Omschrijving = newest.Omschrijving,
                    CatalogusId = OzUrlToGuidConverter.ExtractUuidFromUrl(newest.Catalogus)
                });
        }
    }
}
