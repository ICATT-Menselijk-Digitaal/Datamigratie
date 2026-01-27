using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

public class DocumentPropertyMapping
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public required Guid ZaaktypenMappingId { get; set; }
    
    public ZaaktypenMapping ZaaktypenMapping { get; set; } = null!;
    
    [Required]
    public required string DetPropertyName { get; set; }
    
    [Required]
    public required string DetValue { get; set; }
    
    [Required]
    public required string OzValue { get; set; }
}
