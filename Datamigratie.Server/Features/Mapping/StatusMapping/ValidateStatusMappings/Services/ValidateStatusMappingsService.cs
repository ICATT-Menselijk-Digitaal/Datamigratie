using Datamigratie.Common.Services.Det;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.StatusMapping.ValidateStatusMappings.Services;

public interface IValidateStatusMappingsService
{
    Task<bool> AreAllStatusesMapped(string detZaaktypeId);
}

public class ValidateStatusMappingsService(
    DatamigratieDbContext context,
    IDetApiClient detApiClient) : IValidateStatusMappingsService
{
    public async Task<bool> AreAllStatusesMapped(string detZaaktypeId)
    {
        var detZaaktype = await detApiClient.GetZaaktypeDetail(detZaaktypeId);
        
        if (detZaaktype == null)
            return false;

        var activeDetStatuses = detZaaktype.Statussen
            .Where(s => s.Actief)
            .Select(s => s.Naam)
            .ToList();

        if (activeDetStatuses.Count == 0)
            return true;

        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId);

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
