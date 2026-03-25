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

        var zaaktypenMapping = await dbContext.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId);

        if (zaaktypenMapping == null)
        {
            logger.LogWarning("No zaaktype mapping found for zaaktype {DetZaaktypeId}", safeDetZaaktypeId);
            return (false, []);
        }

        var roltypeMappings = await dbContext.RoltypeMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();

        var missingRollen = MappingConstants.DetRol.Options
            .Where(rol => !roltypeMappings.Any(m => m.DetRol == rol.Id))
            .ToList();

        if (missingRollen.Count != 0)
        {
            logger.LogWarning("Missing roltype mappings for zaaktype {DetZaaktypeId}: {MissingRollen}",
                safeDetZaaktypeId, string.Join(", ", missingRollen.Select(r => r.Id)));
            return (false, []);
        }

        return (true, roltypeMappings.ToDictionary(m => m.DetRol, m => m.OzRoltypeUrl));
    }
}
