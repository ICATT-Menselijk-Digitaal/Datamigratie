using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Resultaat;

public interface IValidateResultaattypeMappingsService
{
    Task<(bool IsValid, Dictionary<string, Guid> Mappings)> ValidateAndGetResultaattypeMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateResultaattypeMappingsService(
    DatamigratieDbContext context) : IValidateResultaattypeMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, Guid> Mappings)> ValidateAndGetResultaattypeMappings(DetZaaktypeDetail detZaaktype)
    {
        var detResultaattypen = detZaaktype.Resultaten?
            .Select(r => r.Resultaat.Naam)
            .ToList() ?? [];

        // If no resultaattypen for det zaaktype, consider it valid
        if (detResultaattypen.Count == 0)
        {
            return (true, new Dictionary<string, Guid>());
        }

        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (zaaktypenMapping == null)
            return (false, new Dictionary<string, Guid>());

        var mappings = await context.ResultaattypeMappings
            .Where(rm => rm.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();

        var mappingDictionary = mappings.ToDictionary(m => m.DetResultaattypeNaam, m => m.OzResultaattypeId);

        // checking if all DET resultaattypen are mapped
        var allMapped = detResultaattypen.All(resultaat => mappingDictionary.ContainsKey(resultaat));
        
        return (allMapped, mappingDictionary);
    }
}
