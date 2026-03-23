using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration.Models;

public class StartSingleMigrationRequest
{
    [Required]
    public required string DetZaaktypeId { get; set; }

    [Required]
    public required string Zaaknummer { get; set; }
}
