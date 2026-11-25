using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

public class PdfInformatieobjecttypeMapping
{
    public Guid Id { get; set; }

    public required Guid ZaaktypeVersieMappingId { get; set; }

    public ZaaktypeVersieMapping ZaaktypeVersieMapping { get; set; } = null!;

    public required Guid OzInformatieobjecttypeId { get; set; }
}
