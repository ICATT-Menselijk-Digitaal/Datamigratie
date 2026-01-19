namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.Resultaattypen.SaveResultaattypeMappings.Models
{
    public class SaveResultaattypeMappingRequest
    {
        public required List<ResultaattypenMappingItem> Mappings { get; set; }
    }

    public class ResultaattypenMappingItem
    {
        public required string DetResultaattypeNaam { get; set; }
        public required Guid OzResultaattypeId { get; set; }
    }
}
