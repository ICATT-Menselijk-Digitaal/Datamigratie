using Datamigratie.Common.Models;

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

        public static class OzVertrouwelijkheidaanduiding
        {
            public static readonly ZaaktypeOptionItem[] Options =
            [
                new() { Value = "openbaar", Label = "Openbaar" },
                new() { Value = "beperkt_openbaar", Label = "Beperkt openbaar" },
                new() { Value = "intern", Label = "Intern" },
                new() { Value = "zaakvertrouwelijk", Label = "Zaakvertrouwelijk" },
                new() { Value = "vertrouwelijk", Label = "Vertrouwelijk" },
                new() { Value = "geheim", Label = "Geheim" },
                new() { Value = "zeer_geheim", Label = "Zeer geheim" }
            ];
        }
    }
}
