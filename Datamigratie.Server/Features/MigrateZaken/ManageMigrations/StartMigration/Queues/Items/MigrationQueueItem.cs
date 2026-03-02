using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues.Items
{
        public class MigrationQueueItem
        {
                public required string DetZaaktypeId { get; set; }

                /// <summary>
                /// Rsin Mapping is validated and set by StartMigrationController before queuing.
                /// It is guaranteed to be non-null and valid when PerformMigrationAsync is called.
                /// </summary>
                public required RsinMapping RsinMapping { get; set; }

                /// <summary>
                /// Status mappings loaded and validated by StartMigrationController before queuing.
                /// Dictionary: DetStatusNaam -> OzStatustypeId
                /// </summary>
                public required Dictionary<string, Guid> StatusMappings { get; set; }

                /// <summary>
                /// Resultaat mappings loaded and validated by StartMigrationController before queuing.
                /// Dictionary: DetResultaattypeNaam -> OzResultaattypeId
                /// </summary>
                public required Dictionary<string, Guid> ResultaatMappings { get; set; }

                /// <summary>
                /// Document status mappings loaded and validated by StartMigrationController before queuing.
                /// Dictionary: DetDocumentstatusNaam -> OzDocumentstatus (e.g., "in_bewerking", "definitief")
                /// </summary>
                public required Dictionary<string, string> DocumentstatusMappings { get; set; }

                /// <summary>
                /// Document property mappings loaded and validated by StartMigrationController before queuing.
                /// Dictionary: PropertyName -> (DetValue -> OzValue)
                /// </summary>
                public required Dictionary<string, Dictionary<string, string>> DocumentPropertyMappings { get; set; }

                /// <summary>
                /// Vertrouwelijkheid mappings loaded and validated by StartMigrationController before queuing.
                /// Dictionary: DetVertrouwelijkheid (true/false) -> OzVertrouwelijkheidaanduiding
                /// </summary>
                public required Dictionary<bool, ZaakVertrouwelijkheidaanduiding> ZaakVertrouwelijkheidMappings { get; set; }

                /// <summary>
                /// Besluittype mappings loaded and validated by StartMigrationController before queuing.
                /// Dictionary: DetBesluittypeNaam -> OzBesluittypeId
                /// </summary>
                public required Dictionary<string, Guid> BesluittypeMappings { get; set; }

                /// <summary>
                /// PDF informatieobjecttype mapping loaded and validated by StartMigrationController before queuing.
                /// The OZ informatieobjecttype ID to assign to the generated PDF document.
                /// </summary>
                public required Guid PdfInformatieobjecttypeId { get; set; }
        }
}
