using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.ShowOzZaaktypeInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.ShowOzZaaktypeInfo
{
    [ApiController]
    [Route("api/oz/zaaktypen")]
    public class ShowOzZaaktypeInfoController(IOpenZaakApiClient openZaakApiClient) : ControllerBase
    {
        [HttpGet("{zaaktypeUuid}")]
        public async Task<ActionResult<EnrichedOzZaaktype>> GetZaaktype(Guid zaaktypeUuid)
        {
            var ozZaaktype = await openZaakApiClient.GetZaaktype(zaaktypeUuid);

            if (ozZaaktype == null)
            {
                return NotFound();
            }

            var ozStatustypes = await openZaakApiClient.GetStatustypesForZaaktype(new Uri(ozZaaktype.Url));
            var ozResultaattypes = await openZaakApiClient.GetResultaattypenForZaaktype(new Uri(ozZaaktype.Url));
            var ozInformatieobjecttypen = await openZaakApiClient.GetInformatieobjecttypenForZaaktype(new Uri(ozZaaktype.Url));
            var ozBesluittypen = await openZaakApiClient.GetBesluittypenForZaaktype(new Uri(ozZaaktype.Url));

            var enrichedOzZaaktype = new EnrichedOzZaaktype
            {
                Url = ozZaaktype.Url,
                Identificatie = ozZaaktype.Identificatie,
                Statustypes = ozStatustypes.OrderBy(st => st.Volgnummer).ToList(),
                Resultaattypen = ozResultaattypes,
                Informatieobjecttypen = ozInformatieobjecttypen,
                Besluittypen = ozBesluittypen,
                Omschrijving = ozZaaktype.Omschrijving,
                OzZaakVertrouwelijkheidaanduidingen = GetZaakVertrouwelijkheidaanduidingOptions(),
                OzDocumentVertrouwelijkheidaanduidingen = GetDocumentVertrouwelijkheidaanduidingOptions()
            };

            return enrichedOzZaaktype;
        }

        private static List<OzZaakVertrouwelijkheidaanduiding> GetZaakVertrouwelijkheidaanduidingOptions()
        {
            return Enum.GetValues<VertrouwelijkheidsAanduiding>()
                .Select(value => new OzZaakVertrouwelijkheidaanduiding
                {
                    Value = value.ToString(),
                    Label = FormatVertrouwelijkheidLabel(value.ToString())
                })
                .ToList();
        }

        private static List<OzDocumentVertrouwelijkheidaanduiding> GetDocumentVertrouwelijkheidaanduidingOptions()
        {
            return Enum.GetValues<VertrouwelijkheidsAanduiding>()
                .Select(value => new OzDocumentVertrouwelijkheidaanduiding
                {
                    Value = value.ToString(),
                    Label = FormatVertrouwelijkheidLabel(value.ToString())
                })
                .ToList();
        }

        private static string FormatVertrouwelijkheidLabel(string value)
        {
            return value switch
            {
                "openbaar" => "Openbaar",
                "beperkt_openbaar" => "Beperkt openbaar",
                "intern" => "Intern",
                "zaakvertrouwelijk" => "Zaakvertrouwelijk",
                "vertrouwelijk" => "Vertrouwelijk",
                "geheim" => "Geheim",
                "zeer_geheim" => "Zeer geheim",
                _ => value
            };
        }
    }
}
