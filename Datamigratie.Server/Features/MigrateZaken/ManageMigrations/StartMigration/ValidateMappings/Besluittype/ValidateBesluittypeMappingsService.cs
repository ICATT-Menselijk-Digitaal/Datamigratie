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

        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (zaaktypenMapping == null)
            return (false, new Dictionary<string, Guid>());

        var mappings = await context.BesluittypeMappings
            .Where(bm => bm.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();

        var mappingDictionary = mappings.ToDictionary(m => m.DetBesluittypeNaam, m => m.OzBesluittypeId);

        // checking if all active DET besluittypen are mapped
        var allMapped = detBesluittypen.All(besluittype => mappingDictionary.ContainsKey(besluittype));

        return (allMapped, mappingDictionary);
    }
}
