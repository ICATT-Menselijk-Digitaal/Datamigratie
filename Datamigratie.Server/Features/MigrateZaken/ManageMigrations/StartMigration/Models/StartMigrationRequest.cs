using System.ComponentModel.DataAnnotations;
using Datamigratie.Data.Entities;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;

public class StartMigrationRequest
{
    [Required]
    public required string DetZaaktypeId { get; set; }

    public MigrationType MigrationType { get; set; } = MigrationType.Full;
}

