using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.SaveStatusMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.SaveStatusMappings.Services;

public interface ISaveStatusMappingsService
{
    Task SaveStatusMappings(Guid zaaktypenMappingId, SaveStatusMappingsRequest request);
}

public class SaveStatusMappingsService(DatamigratieDbContext context) : ISaveStatusMappingsService
{
    public async Task SaveStatusMappings(Guid zaaktypenMappingId, SaveStatusMappingsRequest request)
    {
        var existingMappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();
        
        context.StatusMappings.RemoveRange(existingMappings);

        // TODO DATA-247: take the zaaktype version from the route once mapping per version is in place
        var zaaktypeVersieMappingId = await context.ZaaktypeVersieMappings
            .Where(v => v.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(v => v.Id)
            .SingleAsync();

        var newMappings = request.Mappings.Select(m => new Data.Entities.StatusMapping
        {
            ZaaktypeVersieMappingId = zaaktypeVersieMappingId,
            DetStatusNaam = m.DetStatusNaam,
            OzStatustypeId = m.OzStatustypeId
        });
        
        await context.StatusMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}
