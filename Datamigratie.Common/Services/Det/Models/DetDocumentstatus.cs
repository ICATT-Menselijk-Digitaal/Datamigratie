namespace Datamigratie.Common.Services.Det.Models
{
    public class DetDocumentstatus
    {
        public bool Actief { get; set; }
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
    }
}
