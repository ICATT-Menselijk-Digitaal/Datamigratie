using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartPartialMigration.Models;

public class StartPartialMigrationRequest
{
    [Required]
    public required string DetZaaktypeId { get; set; }
}
