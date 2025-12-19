
namespace Datamigratie.Server.Features.MigrateZaak.Models
{
    public class MigrateZaakMappingModel
    {
        public string? Rsin { get; set; }
        public Guid OpenZaaktypeId { get; internal set; }
    }
}
