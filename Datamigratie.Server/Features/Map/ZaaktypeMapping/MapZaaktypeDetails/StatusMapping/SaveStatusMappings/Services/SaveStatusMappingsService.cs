using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.SaveStatusMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.SaveStatusMappings.Services;

public interface ISaveStatusMappingsService
{
    Task SaveStatusMappings(Guid zaaktypeVersieMappingId, SaveStatusMappingsRequest request);
}

public class SaveStatusMappingsService(DatamigratieDbContext context) : ISaveStatusMappingsService
{
    public async Task SaveStatusMappings(Guid zaaktypeVersieMappingId, SaveStatusMappingsRequest request)
    {
        var existingMappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
            .ToListAsync();
        
        context.StatusMappings.RemoveRange(existingMappings);

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
