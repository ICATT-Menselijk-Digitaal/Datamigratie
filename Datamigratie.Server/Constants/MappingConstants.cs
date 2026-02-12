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
            public static readonly ZaaktypeOptionItem[] Options = [.. Enum.GetValues<ZaakVertrouwelijkheidaanduiding>()
                .Select(value => new ZaaktypeOptionItem
                {
                    Value = value.ToString(),
                    Label = FormatLabel(value)
                })];

            private static string FormatLabel(ZaakVertrouwelijkheidaanduiding value)
            {
                return value switch
                {
                    ZaakVertrouwelijkheidaanduiding.openbaar => "Openbaar",
                    ZaakVertrouwelijkheidaanduiding.beperkt_openbaar => "Beperkt openbaar",
                    ZaakVertrouwelijkheidaanduiding.intern => "Intern",
                    ZaakVertrouwelijkheidaanduiding.zaakvertrouwelijk => "Zaakvertrouwelijk",
                    ZaakVertrouwelijkheidaanduiding.vertrouwelijk => "Vertrouwelijk",
                    ZaakVertrouwelijkheidaanduiding.confidentieel => "Confidentieel",
                    ZaakVertrouwelijkheidaanduiding.geheim => "Geheim",
                    ZaakVertrouwelijkheidaanduiding.zeer_geheim => "Zeer geheim",
                    _ => value.ToString()
                };
            }
        }

        public static class OzDocumentVertrouwelijkheidaanduiding
        {
            public static readonly ZaaktypeOptionItem[] Options = [.. Enum.GetValues<DocumentVertrouwelijkheidaanduiding>()
                .Select(value => new ZaaktypeOptionItem
                {
                    Value = value.ToString(),
                    Label = FormatLabel(value)
                })];

            private static string FormatLabel(DocumentVertrouwelijkheidaanduiding value)
            {
                return value switch
                {
                    DocumentVertrouwelijkheidaanduiding.openbaar => "Openbaar",
                    DocumentVertrouwelijkheidaanduiding.beperkt_openbaar => "Beperkt openbaar",
                    DocumentVertrouwelijkheidaanduiding.intern => "Intern",
                    DocumentVertrouwelijkheidaanduiding.zaakvertrouwelijk => "Zaakvertrouwelijk",
                    DocumentVertrouwelijkheidaanduiding.vertrouwelijk => "Vertrouwelijk",
                    DocumentVertrouwelijkheidaanduiding.confidentieel => "Confidentieel",
                    DocumentVertrouwelijkheidaanduiding.geheim => "Geheim",
                    DocumentVertrouwelijkheidaanduiding.zeer_geheim => "Zeer geheim",
                    _ => value.ToString()
                };
            }
        }
    }
}
