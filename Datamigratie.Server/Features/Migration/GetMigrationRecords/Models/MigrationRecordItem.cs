namespace Datamigratie.Server.Features.Migration.GetMigrationRecords.Models;

public class MigrationRecordItem
{
    public int Id { get; set; }
    public required string DetZaaknummer { get; set; }
    public string? OzZaaknummer { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorTitle { get; set; }
    public string? ErrorDetails { get; set; }
    public int? StatusCode { get; set; }
    public DateTime ProcessedAt { get; set; }
}
