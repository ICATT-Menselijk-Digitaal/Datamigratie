using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Roltype;

public interface IValidateRoltypeMappingsService
{
    Task<(bool IsValid, Dictionary<DetRolType, Uri> Mappings)> ValidateAndGetRoltypeMappings(string detZaaktypeId);
}

public class ValidateRoltypeMappingsService(
    DatamigratieDbContext dbContext,
    ILogger<ValidateRoltypeMappingsService> logger) : IValidateRoltypeMappingsService
{
    public async Task<(bool IsValid, Dictionary<DetRolType, Uri> Mappings)> ValidateAndGetRoltypeMappings(string detZaaktypeId)
    {
        var roltypeMappings = await dbContext.PropertyMappings
            .Where(m => m.ZaaktypenMapping!.DetZaaktypeId == detZaaktypeId && m.Property == "roltype" && m.SourceId != null)
            .ToDictionaryAsync(m => m.SourceId!, m => m.TargetId);

        var enumValues = new List<(DetRolType DetRol, string? OzRoltypeUrl)>();
        var invalidDetRolValues = new List<string?>();

        foreach (var mapping in roltypeMappings)
        {
            if (!Enum.TryParse<DetRolType>(mapping.Key, ignoreCase: true, out var detRol))
            {
                invalidDetRolValues.Add(mapping.Key);
                continue;
            }
            enumValues.Add((detRol, mapping.Value));
        }

        if (invalidDetRolValues.Count != 0)
        {
            logger.LogWarning(
                "Invalid DetRol values for zaaktype {DetZaaktypeId}: {InvalidDetRolValues}",
                detZaaktypeId.ReplaceLineEndings(" "),
                string.Join(", ", invalidDetRolValues.Select(v => v ?? "<null>")));
            return (false, []);
        }

        var missingRollen = Enum.GetValues<DetRolType>()
            .Where(rol => !enumValues.Any(e => e.DetRol == rol))
            .ToList();

        if (missingRollen.Count != 0)
        {
            logger.LogWarning("Missing roltype mappings for zaaktype {DetZaaktypeId}: {MissingRollen}",
                detZaaktypeId.ReplaceLineEndings(" "), string.Join(", ", missingRollen));
            return (false, []);
        }

        return (true, enumValues
            .Where(m => m.OzRoltypeUrl != "alleen_pdf")
            .ToDictionary(m => m.DetRol, m => new Uri(m.OzRoltypeUrl!)));
    }
}
