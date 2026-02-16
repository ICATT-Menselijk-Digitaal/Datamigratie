using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

public class PdfInformatieobjecttypeMapping
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required Guid ZaaktypenMappingId { get; set; }

    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;

    [Required]
    public required Guid OzInformatieobjecttypeId { get; set; }
}
