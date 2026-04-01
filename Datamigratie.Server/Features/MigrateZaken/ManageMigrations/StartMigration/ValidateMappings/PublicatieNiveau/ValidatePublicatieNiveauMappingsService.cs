using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Server.Constants;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PublicatieNiveau;

public interface IValidatePublicatieNiveauMappingsService
{
    Task<(bool IsValid, Dictionary<string, string> Mappings)> ValidateAndGetPublicatieNiveauMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidatePublicatieNiveauMappingsService(
    DatamigratieDbContext dbContext,
    ILogger<ValidatePublicatieNiveauMappingsService> logger) : IValidatePublicatieNiveauMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, string> Mappings)> ValidateAndGetPublicatieNiveauMappings(DetZaaktypeDetail detZaaktype)
    {
        var publicatieNiveauMappings = await dbContext.PropertyMappings
            .Where(m => m.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && m.Property == "publicatieniveau" && m.SourceId != null)
            .ToDictionaryAsync(x => x.SourceId!, x => x.TargetId);

        var missingPublicatieNiveaus = MappingConstants.PublicatieNiveau.Options
            .Where(pn => !publicatieNiveauMappings.Any(m => m.Key == pn.Id))
            .ToList();

        if (missingPublicatieNiveaus.Count != 0)
        {
            logger.LogWarning("Missing publicatieniveau mappings for zaaktype {DetZaaktypeFunctioneleIdentificatie}: {MissingValues}",
                detZaaktype.FunctioneleIdentificatie, string.Join(", ", missingPublicatieNiveaus.Select(p => p.Id)));
            return (false, new Dictionary<string, string>());
        }

        return (true, publicatieNiveauMappings);
    }
}
