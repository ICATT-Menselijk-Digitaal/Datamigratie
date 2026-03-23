namespace Datamigratie.Data.Entities;

public class DocumenttypeMapping
{
    public Guid Id { get; set; }
    public required Guid ZaaktypenMappingId { get; set; }
    public required string DetDocumenttypeNaam { get; set; }
    public required string OzInformatieobjecttypeUrl { get; set; }

    // Navigation property
    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;
}
