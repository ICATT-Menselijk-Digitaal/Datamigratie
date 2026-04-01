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

    /// <summary>
    /// Roltype mappings loaded and validated by StartMigrationController before queuing.
    /// Dictionary: DetRol -> OzRoltypeUrl. Alleen-PDF rollen are excluded (no OZ rol needed).
    /// </summary>
    public required Dictionary<DetRolType, Uri> RoltypeMappings { get; set; }
}
