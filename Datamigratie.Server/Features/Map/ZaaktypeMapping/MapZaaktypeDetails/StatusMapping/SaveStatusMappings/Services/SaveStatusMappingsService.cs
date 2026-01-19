using Datamigratie.Data;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.StatusMapping.SaveStatusMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.StatusMapping.SaveStatusMappings.Services;

public interface ISaveStatusMappingsService
{
    Task SaveStatusMappings(Guid zaaktypenMappingId, SaveStatusMappingsRequest request);
}

public class SaveStatusMappingsService(DatamigratieDbContext context) : ISaveStatusMappingsService
{
    public async Task SaveStatusMappings(Guid zaaktypenMappingId, SaveStatusMappingsRequest request)
    {
        var existingMappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();
        
        context.StatusMappings.RemoveRange(existingMappings);

        var newMappings = request.Mappings.Select(m => new Data.Entities.StatusMapping
        {
            ZaaktypenMappingId = zaaktypenMappingId,
            DetStatusNaam = m.DetStatusNaam,
            OzStatustypeId = m.OzStatustypeId
        });
        
        await context.StatusMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}
