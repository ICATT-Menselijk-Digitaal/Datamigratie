
namespace Datamigratie.Server.Features.MigrateZaak.Models
{
    public class MigrateZaakMappingModel
    {
        public required string Rsin { get; set; }
        public Guid OpenZaaktypeId { get; internal set; }
        public Uri? ResultaattypeUri { get; set; }
        public Uri? StatustypeUri { get; set; }
    }
}
