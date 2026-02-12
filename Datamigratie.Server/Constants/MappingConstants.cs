using Datamigratie.Common.Models;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Constants
{
    public static class MappingConstants
    {
        public static class PublicatieNiveau
        {
            public static readonly ZaaktypeOptionItem[] Options =
            [
                new() { Value = "extern", Label = "Extern" },
                new() { Value = "intern", Label = "Intern" },
                new() { Value = "vertrouwelijk", Label = "Vertrouwelijk" }
            ];
        }

        public static class DetVertrouwelijkheid
        {
            public static readonly ZaaktypeOptionItem[] Options =
            [
                new() { Value = "true", Label = "Ja (Vertrouwelijk)" },
                new() { Value = "false", Label = "Nee (Niet vertrouwelijk)" }
            ];
        }

        public static class OzZaakVertrouwelijkheidaanduiding
        {
            public static readonly ZaaktypeOptionItem[] Options = [.. Enum.GetValues<ZaakVertrouwelijkheidAanduiding>()
                .Select(value => new ZaaktypeOptionItem
                {
                    Value = value.ToString(),
                    Label = FormatLabel(value)
                })];

            private static string FormatLabel(ZaakVertrouwelijkheidAanduiding value)
            {
                return value switch
                {
                    ZaakVertrouwelijkheidAanduiding.openbaar => "Openbaar",
                    ZaakVertrouwelijkheidAanduiding.beperkt_openbaar => "Beperkt openbaar",
                    ZaakVertrouwelijkheidAanduiding.intern => "Intern",
                    ZaakVertrouwelijkheidAanduiding.zaakvertrouwelijk => "Zaakvertrouwelijk",
                    ZaakVertrouwelijkheidAanduiding.vertrouwelijk => "Vertrouwelijk",
                    ZaakVertrouwelijkheidAanduiding.confidentieel => "Confidentieel",
                    ZaakVertrouwelijkheidAanduiding.geheim => "Geheim",
                    ZaakVertrouwelijkheidAanduiding.zeer_geheim => "Zeer geheim",
                    _ => value.ToString()
                };
            }
        }

        public static class OzDocumentVertrouwelijkheidaanduiding
        {
            public static readonly ZaaktypeOptionItem[] Options = [.. Enum.GetValues<DocumentVertrouwelijkheidAanduiding>()
                .Select(value => new ZaaktypeOptionItem
                {
                    Value = value.ToString(),
                    Label = FormatLabel(value)
                })];

            private static string FormatLabel(DocumentVertrouwelijkheidAanduiding value)
            {
                return value switch
                {
                    DocumentVertrouwelijkheidAanduiding.openbaar => "Openbaar",
                    DocumentVertrouwelijkheidAanduiding.beperkt_openbaar => "Beperkt openbaar",
                    DocumentVertrouwelijkheidAanduiding.intern => "Intern",
                    DocumentVertrouwelijkheidAanduiding.zaakvertrouwelijk => "Zaakvertrouwelijk",
                    DocumentVertrouwelijkheidAanduiding.vertrouwelijk => "Vertrouwelijk",
                    DocumentVertrouwelijkheidAanduiding.confidentieel => "Confidentieel",
                    DocumentVertrouwelijkheidAanduiding.geheim => "Geheim",
                    DocumentVertrouwelijkheidAanduiding.zeer_geheim => "Zeer geheim",
                    _ => value.ToString()
                };
            }
        }
    }
}
