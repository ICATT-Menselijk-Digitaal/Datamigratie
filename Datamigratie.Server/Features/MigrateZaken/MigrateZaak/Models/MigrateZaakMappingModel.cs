namespace Datamigratie.Server.Features.Migrate.MigrateZaak.Models
{
    public class MigrateZaakMappingModel
    {
        public required string Rsin { get; set; }
        public Guid OpenZaaktypeId { get; internal set; }
        public Uri? ResultaattypeUri { get; set; }
        public Uri? StatustypeUri { get; set; }

        /// <summary>
        /// Document status mappings: DetDocumentstatusNaam -> OzDocumentstatus (e.g., "in_bewerking", "definitief")
        /// </summary>
        public required Dictionary<string, string> DocumentstatusMappings { get; set; }
    }
}
