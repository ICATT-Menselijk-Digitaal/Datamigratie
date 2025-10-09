using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Server.Features.Migration.StartMigration.Models;

public class StartMigrationRequest
{
    [Required]
    public required string DetZaaktypeId { get; set; }
}

