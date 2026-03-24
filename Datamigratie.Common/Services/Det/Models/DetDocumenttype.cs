namespace Datamigratie.Common.Services.Det.Models
{
    public class DetDocumenttype
    {
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
        public bool Actief { get; set; }
        public string? Documentcategorie { get; set; }
        public string? Publicatieniveau { get; set; }
    }
}
