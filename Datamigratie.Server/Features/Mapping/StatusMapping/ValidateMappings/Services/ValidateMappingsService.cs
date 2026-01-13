using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.StatusMapping.ValidateStatusMappings.Services;

public interface IValidateMappingsService
{
    Task<bool> AreAllStatusesMapped(DetZaaktypeDetail detZaaktype);

    Task<bool> AreAllResultaattypenMapped(DetZaaktypeDetail detZaaktype);
}

public class ValidateMappingsService(
    DatamigratieDbContext context) : IValidateMappingsService
{
    public async Task<bool> AreAllStatusesMapped(DetZaaktypeDetail detZaaktype)
    {
        var activeDetStatuses = detZaaktype.Statussen
            .Where(s => s.Actief)
            .Select(s => s.Naam)
            .ToList();

        if (activeDetStatuses.Count == 0)
            return true;

        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.Id);

        if (zaaktypenMapping == null)
            return false;

        var mappedStatuses = await context.StatusMappings
            .Where(sm => sm.ZaaktypenMappingId == zaaktypenMapping.Id)
            .Select(sm => sm.DetStatusNaam)
            .ToListAsync();

        // checking if all active DET statuses are mapped
        return activeDetStatuses.All(status => mappedStatuses.Contains(status));
    }

    public async Task<bool> AreAllResultaattypenMapped(DetZaaktypeDetail detZaaktype)
    {
        var detResultaattypen = detZaaktype.Resultaten?
            .Select(r => r.Resultaat.Naam)
            .ToList() ?? [];

        if (detResultaattypen.Count == 0)
            return true;

        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.Id);

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
