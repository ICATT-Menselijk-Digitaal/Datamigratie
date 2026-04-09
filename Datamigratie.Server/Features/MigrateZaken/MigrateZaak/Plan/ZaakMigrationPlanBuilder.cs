using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Plan
{
    public static class ZaakMigrationPlanBuilder
    {
        public static ZaakMigrationPlan Build(
            DetZaak detZaak,
            MigrateZaakMappingModel mapping)
        {
            var zaakRequest = mapping.ZaakMapper.Map(detZaak);
            var resultaat = detZaak.Resultaat is { } detResultaat ? mapping.ResultaatMapper.Map(detResultaat) : null;
            var status = detZaak.ZaakStatus is { } detStatus ? mapping.StatusMapper.Map(detStatus, detZaak) : null;
            var pdfDocument = mapping.PdfMapper.Map(detZaak);
            var documents = detZaak.Documenten?.Select(mapping.DocumentMapper.Map).ToList() ?? [];
            var besluiten = detZaak.Besluiten?.Select(mapping.BesluitMapper.Map).ToList() ?? [];
            var rollen = mapping.RolMapper.MapRoles(detZaak).ToList();

            return new ZaakMigrationPlan
            {
                ZaakRequest = zaakRequest,
                Resultaat = resultaat,
                Status = status,
                PdfDocument = pdfDocument,
                Documents = documents,
                Besluiten = besluiten,
                Rollen = rollen
            };
        }
    }
}
