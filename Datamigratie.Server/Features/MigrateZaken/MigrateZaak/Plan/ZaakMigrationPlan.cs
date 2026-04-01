using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Plan
{
    public class ZaakMigrationPlan
    {
        public required CreateOzZaakRequest ZaakRequest { get; init; }
        public CreateOzResultaatRequest? Resultaat { get; init; }
        public CreateOzStatusRequest? Status { get; init; }
        public required OzDocument PdfDocument { get; init; }
        public required IReadOnlyList<DocumentMigrationPlan> Documents { get; init; }
        public required IReadOnlyList<CreateOzBesluitRequest> Besluiten { get; init; }
    }

    public record DocumentMigrationPlan(IReadOnlyList<DocumentVersionPlan> Versions);
    public record DocumentVersionPlan(OzDocument Document, long DocumentInhoudId);
}
