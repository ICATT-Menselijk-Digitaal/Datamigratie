namespace Datamigratie.Common.Services.Det.Models
{
    public class DetBesluittype
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public bool Actief { get; set; }
    }
}