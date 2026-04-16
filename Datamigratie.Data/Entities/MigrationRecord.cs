using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Datamigratie.Data.Entities;

public class MigrationRecord
{
    public const int MaxErrorDetailsLength = 10000;

    [Key]
    public int Id { get; set; }
    
    [Required]
    public int MigrationId { get; set; }
    
    public Migration Migration { get; set; } = null!;
    
    [Required]
    public required string DetZaaknummer { get; set; }
    
    public string? OzZaaknummer { get; set; }
    
    [Required]
    public bool IsSuccessful { get; set; }
    
    public string? ErrorTitle { get; set; }
    
    [MaxLength(MaxErrorDetailsLength)]
    public string? ErrorDetails { get; set; }
    
    public int? StatusCode { get; set; }
    
    [Required]
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
