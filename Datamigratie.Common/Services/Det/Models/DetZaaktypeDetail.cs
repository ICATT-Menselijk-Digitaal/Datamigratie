namespace Datamigratie.Common.Services.Det.Models
{
    public class DetZaaktypeDetail : DetZaaktype
    {
        public List<DetStatus> Statussen { get; set; } = [];
        public List<DetZaaktypeDocumenttype> Documenttypen { get; set; } = [];
        public List<DetBesluittype> Besluittypen { get; set; } = [];
    }
}
