using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues.Items;

public class MigrationQueueItem
{
    public required string DetZaaktypeId { get; set; }

    public required IZakenSelector ZakenSelector { get; set; }

    public required ResultaatMapper ResultaatMapper { get; set; }
    public required StatusMapper StatusMapper { get; set; }
    public required ZaakMapper ZaakMapper { get; set; }
    public required DocumentMapper DocumentMapper { get; set; }
    public required BesluitMapper BesluitMapper { get; set; }
    public required PdfMapper PdfMapper { get; set; }

    public required RolMapper RolMapper { get; set; }
}
