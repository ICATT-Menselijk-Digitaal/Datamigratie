using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.StatusMapping.ValidateMappings.Services;

public interface IValidateStatusMappingsService
{
    Task<bool> AreAllStatusesMapped(DetZaaktypeDetail detZaaktype);
}

public class ValidateStatusMappingsService(
    DatamigratieDbContext context) : IValidateStatusMappingsService
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
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (zaaktypenMapping == null)
            return false;

        var mappedStatuses = await context.StatusMappings
            .Where(sm => sm.ZaaktypenMappingId == zaaktypenMapping.Id)
            .Select(sm => sm.DetStatusNaam)
            .ToListAsync();

        // checking if all active DET statuses are mapped
        return activeDetStatuses.All(status => mappedStatuses.Contains(status));
    }
}
