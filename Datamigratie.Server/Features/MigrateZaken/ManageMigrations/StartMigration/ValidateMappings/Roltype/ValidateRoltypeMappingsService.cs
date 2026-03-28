using Datamigratie.Data;
using Datamigratie.Server.Constants;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Roltype;

public interface IValidateRoltypeMappingsService
{
    Task<(bool IsValid, Dictionary<string, string> Mappings)> ValidateAndGetRoltypeMappings(string detZaaktypeId);
}

public class ValidateRoltypeMappingsService(
    DatamigratieDbContext dbContext,
    ILogger<ValidateRoltypeMappingsService> logger) : IValidateRoltypeMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, string> Mappings)> ValidateAndGetRoltypeMappings(string detZaaktypeId)
    {
        var safeDetZaaktypeId = detZaaktypeId.ReplaceLineEndings(" ");

        var roltypeMappings = await dbContext.PropertyMappings
            .Where(m => m.Mapping!.DetZaaktypeId == detZaaktypeId && m.Property == "roltype" && m.SourceId != null)
            .ToDictionaryAsync(m => m.SourceId!, m => m.TargetId);

        var missingRollen = MappingConstants.DetRol.Options
            .Where(rol => !roltypeMappings.Any(m => m.Key == rol.Id))
            .ToList();

        if (missingRollen.Count != 0)
        {
            logger.LogWarning("Missing roltype mappings for zaaktype {DetZaaktypeId}: {MissingRollen}",
                safeDetZaaktypeId, string.Join(", ", missingRollen.Select(r => r.Id)));
            return (false, []);
        }

        return (true, roltypeMappings);
    }
}
