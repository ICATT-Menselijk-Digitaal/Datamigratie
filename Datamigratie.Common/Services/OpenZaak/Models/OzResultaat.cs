namespace Datamigratie.Common.Services.OpenZaak.Models
{
    public class OzResultaat
    {
        public required string Url { get; set; }
        public required string Zaak { get; set; }
        public required string Resultaattype { get; set; }
        public required string Toelichting { get; set; }
    }

    public class CreateOzResultaatRequest
    {
        public required Uri Zaak { get; set; }
        public required Uri Resultaattype { get; set; }
        public string? Toelichting { get; set; }
    }
}