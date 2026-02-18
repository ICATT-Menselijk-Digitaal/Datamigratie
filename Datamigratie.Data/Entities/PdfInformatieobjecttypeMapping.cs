using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

public class PdfInformatieobjecttypeMapping
{
    public Guid Id { get; set; }

    public required Guid ZaaktypenMappingId { get; set; }

    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;

    public required Guid OzInformatieobjecttypeId { get; set; }
}
