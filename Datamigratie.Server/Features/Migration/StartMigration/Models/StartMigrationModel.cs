using System.ComponentModel.DataAnnotations;
using Datamigratie.Data.Entities;

namespace Datamigratie.Server.Features.Migration.StartMigration.Models;

public class StartMigrationModel
{
    [Required]
    public required string DetZaaktypeId { get; set; }
}

public class StartMigrationRequest
{
    [Required]
    public required string DetZaaktypeId { get; set; }
}

public class StartMigrationResponse
{
    public int MigrationId { get; set; }
    public required string DetZaaktypeId { get; set; }
    public MigrationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}
