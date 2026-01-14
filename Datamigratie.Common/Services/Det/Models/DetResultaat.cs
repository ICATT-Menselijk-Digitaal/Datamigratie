namespace Datamigratie.Common.Services.Det.Models
{
    public class DetResultaat
    {
        public required string Naam { get; set; }

        public required bool Actief { get; set; }

        public string? Omschrijving { get; set; }
    }
}
