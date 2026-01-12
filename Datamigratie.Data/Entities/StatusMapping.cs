using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

public class StatusMapping
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required Guid ZaaktypenMappingId { get; set; }
    
    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;
    
    [Required]
    public required string DetStatusNaam { get; set; }
    
    [Required]
    public required Guid OzStatustypeId { get; set; }
}
