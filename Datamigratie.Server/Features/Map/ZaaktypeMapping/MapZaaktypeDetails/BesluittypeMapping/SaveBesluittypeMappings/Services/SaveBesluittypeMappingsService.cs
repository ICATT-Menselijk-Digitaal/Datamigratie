using Datamigratie.Data;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Services;

public interface ISaveBesluittypeMappingsService
{
    Task SaveBesluittypeMappings(Guid zaaktypenMappingId, SaveBesluittypeMappingsRequest request);
}

public class SaveBesluittypeMappingsService(DatamigratieDbContext context) : ISaveBesluittypeMappingsService
{
    public async Task SaveBesluittypeMappings(Guid zaaktypenMappingId, SaveBesluittypeMappingsRequest request)
    {
        var existingMappings = await context.BesluittypeMappings
            .Where(bm => bm.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();
        
        context.BesluittypeMappings.RemoveRange(existingMappings);

        var newMappings = request.Mappings.Select(m => new Data.Entities.BesluittypeMapping
        {
            ZaaktypenMappingId = zaaktypenMappingId,
            DetBesluittypeNaam = m.DetBesluittypeNaam,
            OzBesluittypeId = m.OzBesluittypeId
        });
        
        await context.BesluittypeMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}
