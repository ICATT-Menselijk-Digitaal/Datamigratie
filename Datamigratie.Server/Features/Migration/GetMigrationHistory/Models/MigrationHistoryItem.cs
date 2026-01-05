namespace Datamigratie.Server.Features.Migration.GetMigrationHistory.Models;

public class MigrationHistoryItem
{
    public int Id { get; set; }
    public required string Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int? TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int SuccessfulRecords { get; set; }
    public int FailedRecords { get; set; }
}
