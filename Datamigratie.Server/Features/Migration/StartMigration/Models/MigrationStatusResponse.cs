using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Datamigratie.Server.Features.Migration.StartMigration.Models
{
    public class MigrationStatusResponse
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ServiceMigrationStatus Status { get; set; }

        public string? DetZaaktypeId { get; set; }
    }
}
