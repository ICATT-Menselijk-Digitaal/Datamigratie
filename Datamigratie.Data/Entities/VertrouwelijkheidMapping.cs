using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

public class VertrouwelijkheidMapping
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required Guid ZaaktypeVersieMappingId { get; set; }

    public ZaaktypeVersieMapping ZaaktypeVersieMapping { get; set; } = null!;

    [Required]
    public required bool DetVertrouwelijkheid { get; set; }

    [Required]
    public required string OzVertrouwelijkheidaanduiding { get; set; }
}
