using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Datamigratie.Data.Entities;

public class MigrationRecord
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int MigrationId { get; set; }
    
    [ForeignKey(nameof(MigrationId))]
    public Migration Migration { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public required string DetZaaknummer { get; set; }
    
    [MaxLength(100)]
    public string? OzZaaknummer { get; set; }
    
    [Required]
    public bool IsSuccessful { get; set; }
    
    [MaxLength(500)]
    public string? ErrorTitle { get; set; }
    
    [MaxLength(2000)]
    public string? ErrorDetails { get; set; }
    
    public int? StatusCode { get; set; }
    
    [Required]
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
