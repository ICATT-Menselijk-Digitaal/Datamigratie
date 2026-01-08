using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.Zaaktypen.ShowZaaktype.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Zaaktypen.ShowZaaktype
{
    [ApiController]
    [Route("api/oz/zaaktypen")]
    public class ShowZaaktypeController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {
        [HttpGet("{zaaktypeId}")]
        public async Task<ActionResult<OzZaaktypeDetails>> GetZaaktype(Guid zaaktypeId)
        {
            return new OzZaaktypeDetails
            {
                Resultaattypen = await openZaakApiClient.GetResultaattypenForZaaktype(zaaktypeId),
            };
        }
    }
    public class OzZaaktypeDetails
    {
        public required List<OzResultaattype> Resultaattypen { get; set; }
    }
}
