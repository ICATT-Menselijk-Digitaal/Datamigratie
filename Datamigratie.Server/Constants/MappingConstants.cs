using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Shared.Models;

namespace Datamigratie.Server.Constants
{
    public static class MappingConstants
    {
        public static class DetRol
        {
            public static readonly ZaaktypeOptionItem[] Options =
            [
                new() { Id = "Initiator",       Name = "Initiator" },
                new() { Id = "Behandelaar",     Name = "Behandelaar" },
                new() { Id = "Belanghebbende",  Name = "Belanghebbende" },
                new() { Id = "Gemachtigde",     Name = "Gemachtigde" },
                new() { Id = "Melder",          Name = "Melder" },
                new() { Id = "Medeaanvrager",   Name = "Medeaanvrager" },
                new() { Id = "Plaatsvervanger", Name = "Plaatsvervanger" },
                new() { Id = "Overig",          Name = "Overig" },
            ];
        }

        public static class PublicatieNiveau
        {
            public static readonly ZaaktypeOptionItem[] Options =
            [
                new() { Id = "extern", Name = "Extern" },
                new() { Id = "intern", Name = "Intern" },
                new() { Id = "vertrouwelijk", Name = "Vertrouwelijk" }
            ];
        }

        public static class DetVertrouwelijkheid
        {
            public static readonly ZaaktypeOptionItem[] Options =
            [
                new() { Id = "true", Name = "Ja (Vertrouwelijk)" },
                new() { Id = "false", Name = "Nee (Niet vertrouwelijk)" }
            ];
        }

        public static class OzZaakVertrouwelijkheidaanduiding
        {
            public static readonly ZaaktypeOptionItem[] Options = [.. Enum.GetValues<ZaakVertrouwelijkheidaanduiding>()
                .Select(value => new ZaaktypeOptionItem
                {
                    Id = value.ToString(),
                    Name = FormatLabel(value)
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
                    Id = value.ToString(),
                    Name = FormatLabel(value)
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
