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

        public static class OzZaakVertrouwelijkheidsaanduiding
        {
            public static readonly ZaaktypeOptionItem[] Options = [.. Enum.GetValues<ZaakVertrouwelijkheidsAanduiding>()
                .Select(value => new ZaaktypeOptionItem
                {
                    Value = value.ToString(),
                    Label = FormatLabel(value)
                })];

            private static string FormatLabel(ZaakVertrouwelijkheidsAanduiding value)
            {
                return value switch
                {
                    ZaakVertrouwelijkheidsAanduiding.openbaar => "Openbaar",
                    ZaakVertrouwelijkheidsAanduiding.beperkt_openbaar => "Beperkt openbaar",
                    ZaakVertrouwelijkheidsAanduiding.intern => "Intern",
                    ZaakVertrouwelijkheidsAanduiding.zaakvertrouwelijk => "Zaakvertrouwelijk",
                    ZaakVertrouwelijkheidsAanduiding.vertrouwelijk => "Vertrouwelijk",
                    ZaakVertrouwelijkheidsAanduiding.confidentieel => "Confidentieel",
                    ZaakVertrouwelijkheidsAanduiding.geheim => "Geheim",
                    ZaakVertrouwelijkheidsAanduiding.zeer_geheim => "Zeer geheim",
                    _ => value.ToString()
                };
            }
        }

        public static class OzDocumentVertrouwelijkheidsaanduiding
        {
            public static readonly ZaaktypeOptionItem[] Options = [.. Enum.GetValues<DocumentVertrouwelijkheidsAanduiding>()
                .Select(value => new ZaaktypeOptionItem
                {
                    Value = value.ToString(),
                    Label = FormatLabel(value)
                })];

            private static string FormatLabel(DocumentVertrouwelijkheidsAanduiding value)
            {
                return value switch
                {
                    DocumentVertrouwelijkheidsAanduiding.openbaar => "Openbaar",
                    DocumentVertrouwelijkheidsAanduiding.beperkt_openbaar => "Beperkt openbaar",
                    DocumentVertrouwelijkheidsAanduiding.intern => "Intern",
                    DocumentVertrouwelijkheidsAanduiding.zaakvertrouwelijk => "Zaakvertrouwelijk",
                    DocumentVertrouwelijkheidsAanduiding.vertrouwelijk => "Vertrouwelijk",
                    DocumentVertrouwelijkheidsAanduiding.confidentieel => "Confidentieel",
                    DocumentVertrouwelijkheidsAanduiding.geheim => "Geheim",
                    DocumentVertrouwelijkheidsAanduiding.zeer_geheim => "Zeer geheim",
                    _ => value.ToString()
                };
            }
        }
    }
}
