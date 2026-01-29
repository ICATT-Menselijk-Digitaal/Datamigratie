using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

public class BesluittypeMapping
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required Guid ZaaktypenMappingId { get; set; }
    
    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;
    
    [Required]
    public required string DetBesluittypeNaam { get; set; }
    
    [Required]
    public required Guid OzBesluittypeId { get; set; }
}
