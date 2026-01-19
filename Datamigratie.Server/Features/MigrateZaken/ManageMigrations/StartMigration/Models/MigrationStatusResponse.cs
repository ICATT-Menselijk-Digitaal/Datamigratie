using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models
{
    public class MigrationStatusResponse
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ServiceMigrationStatus Status { get; set; }

        public string? DetZaaktypeId { get; set; }
    }
}
