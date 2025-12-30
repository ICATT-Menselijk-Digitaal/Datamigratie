namespace Datamigratie.Common.Services.Det.Models
{
    public class DetZaaktypeDetail : DetZaaktype
    {
        public List<DetStatus> Statussen { get; set; } = [];
    }
}
