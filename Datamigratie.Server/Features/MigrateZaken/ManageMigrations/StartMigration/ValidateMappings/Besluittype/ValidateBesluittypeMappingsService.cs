using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Besluittype;

public interface IValidateBesluittypeMappingsService
{
    Task<(bool IsValid, Dictionary<string, Guid> Mappings)> ValidateAndGetBesluittypeMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateBesluittypeMappingsService(
    DatamigratieDbContext context) : IValidateBesluittypeMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, Guid> Mappings)> ValidateAndGetBesluittypeMappings(DetZaaktypeDetail detZaaktype)
    {
        var detBesluittypen = detZaaktype.Besluiten
            .Select(b => b.Besluittype.Naam)
            .ToList();

        // If no active besluittypen, consider it valid
        if (detBesluittypen.Count == 0)
        {
            return (true, new Dictionary<string, Guid>());
        }

        var mappings = await context.PropertyMappings
            .Where(bm => bm.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && bm.Property == "besluittype")
            .ToListAsync();

        var mappingDictionary = mappings.ToDictionary(x => x.SourceId, x => Guid.Parse(x.TargetId));

        // checking if all active DET besluittypen are mapped
        var allMapped = detBesluittypen.All(mappingDictionary.ContainsKey);

        return (allMapped, mappingDictionary);
    }
}
