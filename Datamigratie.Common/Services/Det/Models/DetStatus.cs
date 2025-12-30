namespace Datamigratie.Common.Services.Det.Models
{
    public class DetStatus
    {
        public bool Actief { get; set; }
        public required string Naam { get; set; }
        public required string Omschrijving { get; set; }
        public bool Eind { get; set; }
        public required string ExterneNaam { get; set; }
        public bool Start { get; set; }
        public required string Uitwisselingscode { get; set; }
    }
}
