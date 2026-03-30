using Datamigratie.Data;
using Datamigratie.Server.Constants;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Roltype;

public interface IValidateRoltypeMappingsService
{
    Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetRoltypeMappings(string detZaaktypeId);
}

public class ValidateRoltypeMappingsService(
    DatamigratieDbContext dbContext,
    ILogger<ValidateRoltypeMappingsService> logger) : IValidateRoltypeMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetRoltypeMappings(string detZaaktypeId)
    {
        var roltypeMappings = await dbContext.RoltypeMappings
            .Where(m => m.ZaaktypenMapping.DetZaaktypeId == detZaaktypeId)
            .ToListAsync();

        var missingRollen = MappingConstants.DetRol.Options
            .Where(rol => !roltypeMappings.Any(m => m.DetRol == rol.Id))
            .ToList();

        if (missingRollen.Count != 0)
        {
            logger.LogWarning("Missing roltype mappings for zaaktype {DetZaaktypeId}: {MissingRollen}",
                detZaaktypeId.ReplaceLineEndings(" "), string.Join(", ", missingRollen.Select(r => r.Id)));
            return (false, []);
        }

        return (true, roltypeMappings
            .Where(m => !m.AlleenPdf)
            .ToDictionary(m => m.DetRol, m => new Uri(m.OzRoltypeUrl!)));
    }
}
