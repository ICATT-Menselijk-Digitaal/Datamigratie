using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

public class MigrationTracker
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public Guid ZaaktypeId { get; set; }
    
    [Required]
    public MigrationStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? StartedAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
    
    public int TotalRecords { get; set; }
    
    public int ProcessedRecords { get; set; }
    
    public int SuccessfulRecords { get; set; }
    
    public int FailedRecords { get; set; }
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public enum MigrationStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Cancelled
}
