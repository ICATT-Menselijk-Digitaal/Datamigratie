using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;

public class StartMigrationRequest
{
    [Required]
    public required string DetZaaktypeId { get; set; }
}

