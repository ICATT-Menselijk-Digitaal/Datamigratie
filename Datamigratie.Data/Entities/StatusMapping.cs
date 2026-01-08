using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

public class StatusMapping
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required string DetZaaktypeId { get; set; }
    
    [Required]
    public required string DetStatusNaam { get; set; }
    
    [Required]
    public required Guid OzStatustypeId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
