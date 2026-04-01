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
        var roltypeMappings = await dbContext.RoltypeMappings
            .Where(m => m.ZaaktypenMapping.DetZaaktypeId == detZaaktypeId)
            .ToListAsync();

        var enumValues = roltypeMappings.Select(x => new
        {
            DetRol = Enum.Parse<DetRolType>(x.DetRol, true),
            x.OzRoltypeUrl,
            x.AlleenPdf
        }).DistinctBy(x => x.DetRol).ToList();

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
            .Where(m => !m.AlleenPdf && m.OzRoltypeUrl is not null)
            .ToDictionary(m => m.DetRol, m => new Uri(m.OzRoltypeUrl!)));
    }
}
