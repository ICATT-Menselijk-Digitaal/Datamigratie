using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Models;

public class StartMigrationRequest
{
    [Required]
    public required string DetZaaktypeId { get; set; }
}
