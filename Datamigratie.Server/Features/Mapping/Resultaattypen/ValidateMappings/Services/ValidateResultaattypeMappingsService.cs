using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.Resultaattypen.ValidateMappings.Services;

public interface IValidateResultaattypeMappingsService
{
    Task<bool> AreAllResultaattypenMapped(DetZaaktypeDetail detZaaktype);
}

public class ValidateResultaattypeMappingsService(
    DatamigratieDbContext context) : IValidateResultaattypeMappingsService
{
    public async Task<bool> AreAllResultaattypenMapped(DetZaaktypeDetail detZaaktype)
    {
        var detResultaattypen = detZaaktype.Resultaten?
            .Select(r => r.Resultaat.Naam)
            .ToList() ?? [];

        if (detResultaattypen.Count == 0)
            return true;

        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (zaaktypenMapping == null)
            return false;

        var mappedResultaattypen = await context.ResultaattypeMappings
            .Where(rm => rm.ZaaktypenMappingId == zaaktypenMapping.Id)
            .Select(rm => rm.DetResultaattypeNaam)
            .ToListAsync();

        // checking if all DET resultaattypen are mapped
        return detResultaattypen.All(resultaat => mappedResultaattypen.Contains(resultaat));
    }
}
