namespace Datamigratie.Data.Entities;

public class PublicatieNiveauMapping
{
    public Guid Id { get; set; }
    public required Guid ZaaktypenMappingId { get; set; }
    public required string DetPublicatieNiveau { get; set; }
    public required string OzVertrouwelijkheidaanduiding { get; set; }

    // Navigation property
    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;
}
