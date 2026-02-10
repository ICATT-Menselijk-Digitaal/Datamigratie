using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.Migrate.MigrateZaak.Models
{
        public class MigrateZaakMappingModel
        {
                public required string Rsin { get; set; }
                public Guid OpenZaaktypeId { get; internal set; }
                public Uri? ResultaattypeUri { get; set; }
                public Uri? StatustypeUri { get; set; }
                public Uri? StatustypeUri { get; set; }

                /// <summary>
                /// Document status mappings: DetDocumentstatusNaam -> OzDocumentstatus (e.g., "in_bewerking", "definitief")
                /// </summary>
                public required Dictionary<string, string> DocumentstatusMappings { get; set; }
                public Dictionary<string, Dictionary<string, string>> DocumentPropertyMappings { get; set; } = new();

                /// <summary>
                /// Vertrouwelijkheid mappings: DetVertrouwelijkheid (true/false) -> OzVertrouwelijkheidaanduiding
                /// </summary>
                public required Dictionary<bool, VertrouwelijkheidsAanduiding> VertrouwelijkheidMappings { get; set; }

                /// <summary>
                /// Besluittype mappings: DetBesluittypeNaam -> OzBesluittypeId
                /// </summary>
                public required Dictionary<string, Guid> BesluittypeMappings { get; set; }
        }
}
