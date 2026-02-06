using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.GetVertrouwelijkheidaanduidingOptions.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.GetVertrouwelijkheidaanduidingOptions;

[ApiController]
[Route("api/oz/options/vertrouwelijkheidaanduiding")]
public class GetVertrouwelijkheidaanduidingOptionsController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<VertrouwelijkheidaanduidingOption>> GetVertrouwelijkheidaanduidingOptions()
    {
        var options = Enum.GetValues<VertrouwelijkheidsAanduiding>()
            .Select(value => new VertrouwelijkheidaanduidingOption
            {
                Value = value.ToString(),
                Label = FormatLabel(value.ToString())
            })
            .ToList();

        return Ok(options);
    }

    private static string FormatLabel(string value)
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
            "confidentieel" => "Confidentieel",
            _ => value
        };
    }
}
